using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;
using WalletPlus.Common.Exceptions;
using WalletPlus.Common.IRepository;
using WalletPlus.Common.IService;
using WalletPlus.Common.Models;

namespace WalletPlus.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<TransactionModel> FundWallet(FundWalletModel fundWalletModel)
        {
            var transaction = _mapper.Map<Transaction>(fundWalletModel);

            transaction.Id = Guid.NewGuid();
            transaction.Reference = Guid.NewGuid().ToString().ToUpper();
            transaction.Type = TransactionTypeEnum.Credit;

            await _unitOfWork.Transactions.Insert(transaction);

            //update wallet
            var transactionalWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == fundWalletModel.CustomerId && c.Type == WalletTypeEnum.Transactional);
            if (transactionalWallet == null)
            {
                _logger.LogError($"Unable to load transactional wallet at this time.");
                throw new NotFoundException($"Unable to load transactional wallet at this time.");
            }

            if (transactionalWallet.Amount >= 100000)
            {
                throw new WalletException($"Transactional wallet has reach it maximum limit.");
            }

            if (transactionalWallet.Amount + fundWalletModel.Amount > 100000)
            {
                throw new WalletException($"The amount will exceed the limit of this transactional wallet.");
            }

            transactionalWallet.Amount += fundWalletModel.Amount;
            transactionalWallet.UpdatedDate = DateTime.Now;
            _unitOfWork.Wallets.Update(transactionalWallet);

            var pointSettings = await _unitOfWork.PointSettings.GetAll();
            PointSetting matchPointSetting = null;

            foreach (var pointSetting in pointSettings)
            {
                if (pointSetting.LowAmount <= fundWalletModel.Amount && pointSetting.HighAmount == null)
                {
                    matchPointSetting = pointSetting;
                    break;
                }
                else if (pointSetting.LowAmount <= fundWalletModel.Amount && pointSetting.HighAmount >= fundWalletModel.Amount)
                {
                    matchPointSetting = pointSetting;
                    break;
                }
            }

            //var matchPointSetting = await _unitOfWork.PointSettings.Get(c => c.LowAmount <= fundWalletModel.Amount && c.HighAmount >= fundWalletModel.Amount);
            if (matchPointSetting != null)
            {
                var pointWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == fundWalletModel.CustomerId && c.Type == WalletTypeEnum.Point);
                if (pointWallet == null)
                {
                    _logger.LogError($"Unable to load point wallet at this time.");
                    throw new NotFoundException($"Unable to load point wallet at this time.");
                }

                var pointEarn = (fundWalletModel.Amount * matchPointSetting.Point) / 100m;
                pointEarn = pointEarn > 5000 ? 5000 : pointEarn;
                pointWallet.Amount += pointEarn;

                pointWallet.UpdatedDate = DateTime.Now;
                _unitOfWork.Wallets.Update(pointWallet);

                var point = new Point
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    PointEarn = matchPointSetting.Point
                };

                await _unitOfWork.Points.Insert(point);
            }

            await _unitOfWork.Save();

            return _mapper.Map<TransactionModel>(transaction);
        }

        public async Task<TransactionModel> GetTransaction(Guid id)
        {
            var transaction = await _unitOfWork.Transactions.Get(q => q.Id == id, include: q => q.Include(x => x.Customer).Include(x => x.BeneficiaryCustomer));
            return _mapper.Map<TransactionModel>(transaction);
        }

        public async Task<IList<TransactionModel>> GetTransactions(RequestParams requestParams)
        {
            var transactions = await _unitOfWork.Transactions.GetPagedList(requestParams, include: q => q.Include(x => x.Customer).Include(x => x.BeneficiaryCustomer));
            return _mapper.Map<IList<TransactionModel>>(transactions);
        }

        public async Task<TransactionModel> MakeTransfer(MakeTransferModel makeTransferModel)
        {
            var transaction = _mapper.Map<Transaction>(makeTransferModel);
            transaction.Id = Guid.NewGuid();
            transaction.Reference = Guid.NewGuid().ToString().ToUpper();
            transaction.Type = TransactionTypeEnum.Debit;

            await _unitOfWork.Transactions.Insert(transaction);

            //update wallet
            var sourceTransactionalWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == makeTransferModel.CustomerId && c.Type == WalletTypeEnum.Transactional);
            if (sourceTransactionalWallet == null)
            {
                _logger.LogError($"Unable to load source wallet at this time.");
                throw new NotFoundException($"Unable to load source wallet at this time.");
            }

            if (sourceTransactionalWallet.Amount < makeTransferModel.Amount)
            {
                throw new WalletException($"Not enough amount in source wallet..");
            }

            sourceTransactionalWallet.Amount -= makeTransferModel.Amount;
            _unitOfWork.Wallets.Update(sourceTransactionalWallet);

            var destinationTransactionalWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == makeTransferModel.BeneficiaryCustomerId && c.Type == WalletTypeEnum.Transactional);
            if (destinationTransactionalWallet == null)
            {
                _logger.LogError($"Unable to load destination wallet at this time.");
                throw new NotFoundException($"Unable to load destination wallet at this time.");
            }

            if (destinationTransactionalWallet.Amount >= 100000)
            {
                throw new WalletException($"destination wallet has reach it maximum limit.");
            }

            if (destinationTransactionalWallet.Amount + makeTransferModel.Amount > 100000)
            {
                throw new WalletException($"The amount will exceed the limit of the destination wallet.");
            }

            destinationTransactionalWallet.Amount += makeTransferModel.Amount;
            destinationTransactionalWallet.UpdatedDate = DateTime.Now;
            _unitOfWork.Wallets.Update(destinationTransactionalWallet);

            var pointSettings = await _unitOfWork.PointSettings.GetAll();
            PointSetting matchPointSetting = null;

            foreach (var pointSetting in pointSettings)
            {
                if (pointSetting.LowAmount <= makeTransferModel.Amount && pointSetting.HighAmount == null)
                {
                    matchPointSetting = pointSetting;
                    break;
                }
                else if (pointSetting.LowAmount <= makeTransferModel.Amount && pointSetting.HighAmount >= makeTransferModel.Amount)
                {
                    matchPointSetting = pointSetting;
                    break;
                }
            }

            //var matchPointSetting = await _unitOfWork.PointSettings.Get(c => c.LowAmount <= makeTransferModel.Amount && c.HighAmount >= makeTransferModel.Amount);
            if (matchPointSetting != null)
            {
                var pointWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == makeTransferModel.BeneficiaryCustomerId && c.Type == WalletTypeEnum.Point);
                if (pointWallet == null)
                {
                    _logger.LogError($"Unable to load destination point wallet at this time.");
                    throw new NotFoundException($"Unable to load destination point wallet at this time.");
                }

                var pointEarn = (makeTransferModel.Amount * matchPointSetting.Point) / 100m;
                pointEarn = pointEarn > 5000 ? 5000 : pointEarn;
                pointWallet.Amount += pointEarn;

                pointWallet.UpdatedDate = DateTime.Now;
                _unitOfWork.Wallets.Update(pointWallet);

                var point = new Point
                {
                    Id = Guid.NewGuid(),
                    TransactionId = transaction.Id,
                    PointEarn = matchPointSetting.Point
                };

                await _unitOfWork.Points.Insert(point);
            }

            await _unitOfWork.Save();

            return _mapper.Map<TransactionModel>(transaction);
        }
    }
}

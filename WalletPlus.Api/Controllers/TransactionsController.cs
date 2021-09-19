using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;
using WalletPlus.Common.IRepository;
using WalletPlus.Common.Models;

namespace WalletPlus.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IMapper _mapper;

        public TransactionsController(IUnitOfWork unitOfWork, ILogger<TransactionsController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions([FromQuery] RequestParams requestParams)
        {
            var transactions = await _unitOfWork.Transactions.GetPagedList(requestParams, include: q => q.Include(x => x.Customer).Include(x => x.BeneficiaryCustomer));
            var results = _mapper.Map<IList<TransactionModel>>(transactions);
            return Ok(results);
        }

        [HttpGet("{id:Guid}", Name = "GetTransaction")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransaction(Guid id)
        {
            var transaction = await _unitOfWork.Transactions.Get(q => q.Id == id, include: q => q.Include(x => x.Customer).Include(x => x.BeneficiaryCustomer));
            var result = _mapper.Map<TransactionModel>(transaction);
            return Ok(result);
        }

        [HttpPost("fundwallet")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FundWallet([FromBody] FundWalletModel fundWalletModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid transaction attempt in {nameof(FundWallet)}");
                return BadRequest(ModelState);
            }

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
                return NotFound($"Unable to load transactional wallet at this time.");
            }

            if(transactionalWallet.Amount >= 100000)
            {
                return BadRequest($"Transactional wallet has reach it maximum limit.");
            }

            if (transactionalWallet.Amount + fundWalletModel.Amount > 100000)
            {
                return BadRequest($"The amount will exceed the limit of this transactional wallet.");
            }

            transactionalWallet.Amount += fundWalletModel.Amount;
            transactionalWallet.UpdatedDate = DateTime.Now;
            _unitOfWork.Wallets.Update(transactionalWallet);

            var pointSettings = await _unitOfWork.PointSettings.GetAll();
            PointSetting matchPointSetting = null;

            foreach (var pointSetting in pointSettings)
            {
                if(pointSetting.LowAmount <= fundWalletModel.Amount && pointSetting.HighAmount == null)
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
                    return NotFound($"Unable to load point wallet at this time.");
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

            return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);

        }

        [HttpPost("maketransfer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MakeTransfer([FromBody] MakeTransferModel makeTransferModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid transaction attempt in {nameof(MakeTransfer)}");
                return BadRequest(ModelState);
            }

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
                return NotFound($"Unable to load source wallet at this time.");
            }

            if (sourceTransactionalWallet.Amount < makeTransferModel.Amount)
            {
                return BadRequest($"Not enough amount in source wallet..");
            }

            sourceTransactionalWallet.Amount -= makeTransferModel.Amount;
            _unitOfWork.Wallets.Update(sourceTransactionalWallet);

            var destinationTransactionalWallet = await _unitOfWork.Wallets.Get(c => c.CustomerId == makeTransferModel.BeneficiaryCustomerId && c.Type == WalletTypeEnum.Transactional);
            if (destinationTransactionalWallet == null)
            {
                _logger.LogError($"Unable to load destination wallet at this time.");
                return NotFound($"Unable to load destination wallet at this time.");
            }

            if (destinationTransactionalWallet.Amount >= 100000)
            {
                return BadRequest($"destination wallet has reach it maximum limit.");
            }

            if (destinationTransactionalWallet.Amount + makeTransferModel.Amount > 100000)
            {
                return BadRequest($"The amount will exceed the limit of the destination wallet.");
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
                    return NotFound($"Unable to load destination point wallet at this time.");
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

            return CreatedAtRoute("GetTransaction", new { id = transaction.Id }, transaction);

        }
    }
}

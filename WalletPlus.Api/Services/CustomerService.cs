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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CustomerModel> CreateCustomer(CreateCustomerModel createCustomerModel)
        {
            var customer = _mapper.Map<Customer>(createCustomerModel);

            customer.Id = Guid.NewGuid();
            customer.Active = true;

            await _unitOfWork.Customers.Insert(customer);

            //create wallet
            var transactionWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = WalletTypeEnum.Transactional,
                Amount = 0,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Wallets.Insert(transactionWallet);

            var pointWallet = new Wallet
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                Type = WalletTypeEnum.Point,
                Amount = 0,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Wallets.Insert(pointWallet);

            await _unitOfWork.Save();

            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task<IList<CustomerModel>> GetCustomers(RequestParams requestParams)
        {
            var customers = await _unitOfWork.Customers.GetPagedList(requestParams, include: q => q.Include(x => x.Wallets).Include(x => x.Transactions));
            return _mapper.Map<IList<CustomerModel>>(customers);
        }

        public async Task<CustomerModel> GetCustomer(Guid id)
        {
            var customer = await _unitOfWork.Customers.Get(q => q.Id == id, include: q => q.Include(x => x.Wallets).Include(x => x.Transactions));
            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task<IList<TransactionModel>> GetCustomerTransactions(Guid customerId)
        {
            var transactions = await _unitOfWork.Transactions.GetAll(c => c.CustomerId == customerId || c.BeneficiaryCustomerId == customerId, include: q => q.Include(x => x.Customer).Include(x => x.BeneficiaryCustomer));
            return _mapper.Map<IList<TransactionModel>>(transactions);
        }

        public async Task UpdateCustomer(Guid id, UpdateCustomerModel updateCustomerModel)
        {
            var customer = await _unitOfWork.Customers.Get(q => q.Id == id);
            if (customer == null)
            {
                throw new WalletException("Submitted data is invalid");
            }

            _mapper.Map(updateCustomerModel, customer);
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.Save();
        }
    }
}

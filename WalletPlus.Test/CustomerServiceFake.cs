using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;
using WalletPlus.Common.IService;
using WalletPlus.Common.Models;

namespace WalletPlus.Test
{
    public class CustomerServiceFake : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly List<Customer> _customers;

        public CustomerServiceFake(IMapper mapper)
        {
            _mapper = mapper;

            _customers = new List<Customer>()
            {
                new Customer
                {
                    Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200"),
                    Name = "Test1 Test1",
                    Active = true
                },
                new Customer
                {
                    Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                    Name = "Test2 Test2",
                    Active = true
                }
            };
        }

        public async Task<CustomerModel> CreateCustomer(CreateCustomerModel createCustomerModel)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = createCustomerModel.Name,
                Active = true
            };

            _customers.Add(customer);
            return _mapper.Map<CustomerModel>(customer); ;
        }

        public Task<CustomerModel> GetCustomer(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<CustomerModel>> GetCustomers(RequestParams requestParams)
        {
            return _mapper.Map<IList<CustomerModel>>(_customers);
        }

        public Task<IList<TransactionModel>> GetCustomerTransactions(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCustomer(Guid id, UpdateCustomerModel updateCustomerModel)
        {
            throw new NotImplementedException();
        }
    }
}

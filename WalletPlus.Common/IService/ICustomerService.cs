using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlus.Common.Models;

namespace WalletPlus.Common.IService
{
    public interface ICustomerService
    {
        Task<IList<CustomerModel>> GetCustomers(RequestParams requestParams);
        Task<CustomerModel> GetCustomer(Guid id);
        Task<IList<TransactionModel>> GetCustomerTransactions(Guid customerId);
        Task<CustomerModel> CreateCustomer(CreateCustomerModel createCustomerModel);
        Task UpdateCustomer(Guid id, UpdateCustomerModel updateCustomerModel);
    }
}

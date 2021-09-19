using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlus.Common.Models;

namespace WalletPlus.Common.IService
{
    public interface ITransactionService
    {
        Task<IList<TransactionModel>> GetTransactions(RequestParams requestParams);
        Task<TransactionModel> GetTransaction(Guid id);
        Task<TransactionModel> FundWallet(FundWalletModel fundWalletModel);
        Task<TransactionModel> MakeTransfer(MakeTransferModel makeTransferModel);
    }
}

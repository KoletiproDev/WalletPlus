using System;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;

namespace WalletPlus.Common.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Point> Points { get; }
        IGenericRepository<PointSetting> PointSettings { get; }
        IGenericRepository<Transaction> Transactions { get; }
        IGenericRepository<Wallet> Wallets { get; }

        Task Save();
    }
}

using WalletPlus.Common;
using WalletPlus.Common.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        private IGenericRepository<Customer> _customers;
        private IGenericRepository<Point> _points;
        private IGenericRepository<PointSetting> _pointSettings;
        private IGenericRepository<Transaction> _transactions;
        private IGenericRepository<Wallet> _wallets;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }
        public IGenericRepository<Customer> Customers => _customers ??= new GenericRepository<Customer>(_context);
        public IGenericRepository<Point> Points => _points ??= new GenericRepository<Point>(_context);
        public IGenericRepository<PointSetting> PointSettings => _pointSettings ??= new GenericRepository<PointSetting>(_context);
        public IGenericRepository<Transaction> Transactions => _transactions ??= new GenericRepository<Transaction>(_context);
        public IGenericRepository<Wallet> Wallets => _wallets ??= new GenericRepository<Wallet>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

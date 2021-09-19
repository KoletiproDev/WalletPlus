using Microsoft.EntityFrameworkCore;
using WalletPlus.Data.Configurations;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        { }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointSetting> PointSettings { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CustomerConfiguration());
            builder.ApplyConfiguration(new PointConfiguration());
            builder.ApplyConfiguration(new PointSettingConfiguration());
            builder.ApplyConfiguration(new TransactionConfiguration());
            builder.ApplyConfiguration(new WalletConfiguration());
        }
    }
}

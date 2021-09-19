using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Address).IsRequired(false).HasMaxLength(300);
            builder.Property(x => x.PhoneNumber).IsRequired(false).HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired(false).HasMaxLength(256);

            builder.HasMany(x => x.Wallets).WithOne(x => x.Customer).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Transactions).WithOne(x => x.Customer).OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Customers");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            //builder.HasOne(x => x.Customer).WithMany(x => x.Transactions).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.BeneficiaryCustomer).WithMany().OnDelete(DeleteBehavior.Restrict);

            //indexes
            builder.HasIndex(x => x.Reference).IsUnique();

            builder.ToTable("Transactions");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data.Configurations
{
    public class PointConfiguration : IEntityTypeConfiguration<Point>
    {
        public void Configure(EntityTypeBuilder<Point> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            //indexes
            builder.HasIndex(x => x.TransactionId).IsUnique();


            builder.ToTable("Points");
        }
    }
}

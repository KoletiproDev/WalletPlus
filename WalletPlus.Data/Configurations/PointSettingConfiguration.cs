using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletPlus.Common.Entities;

namespace WalletPlus.Data.Configurations
{
    public class PointSettingConfiguration : IEntityTypeConfiguration<PointSetting>
    {
        public void Configure(EntityTypeBuilder<PointSetting> builder)
        {
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

            //indexes
            builder.HasIndex(x => new { x.LowAmount, x.HighAmount}).IsUnique();


            builder.ToTable("PointSettings");
        }
    }
}

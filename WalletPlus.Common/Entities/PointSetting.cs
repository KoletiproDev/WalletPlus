using System;

namespace WalletPlus.Common.Entities
{
    public class PointSetting
    {
        public Guid Id { get; set; }

        public decimal LowAmount { get; set; }

        public decimal HighAmount { get; set; }

        public decimal Point { get; set; }
    }
}

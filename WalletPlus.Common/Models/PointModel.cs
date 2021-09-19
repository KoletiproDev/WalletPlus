using System;

namespace WalletPlus.Common.Models
{

    public class PointModel
    {
        public Guid Id { get; set; }

        public TransactionModel Transaction { get; set; }

        public decimal PointEarn { get; set; }
    }
}

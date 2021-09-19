using System;

namespace WalletPlus.Common.Entities
{
    public class Point
    {
        public Guid Id { get; set; }

        public Guid TransactionId { get; set; }
       
        public decimal PointEarn { get; set; }

        public Transaction Transaction { get; set; }
    }
}

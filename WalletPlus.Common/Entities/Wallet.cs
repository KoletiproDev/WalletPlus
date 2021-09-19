using System;

namespace WalletPlus.Common.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
       
        public WalletTypeEnum Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Customer Customer { get; set; }
    }
}

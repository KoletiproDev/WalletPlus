using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;

namespace WalletPlus.Common.Models
{
    
    public class FundWalletModel
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }

    public class MakeTransferModel
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public Guid BeneficiaryCustomerId { get; set; }
    }

    public class TransactionModel
    {
        public Guid Id { get; set; }

        public CustomerModel Customer { get; set; }

        public TransactionTypeEnum Type { get; set; }

        public decimal Amount { get; set; }

        public CustomerModel BeneficiaryCustomer { get; set; }

        public DateTime Date { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WalletPlus.Common.Models
{
    
    public class CreateCustomerModel
    {
        [Required]
        [StringLength(maximumLength: 200, ErrorMessage = "Customer name is too long")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
    public class UpdateCustomerModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(maximumLength: 200, ErrorMessage = "Customer name is too long")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }
    }

    public class CustomerModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public IList<WalletModel> Wallets { get; set; }
        public IList<TransactionModel> Transactions { get; set; }
    }
}

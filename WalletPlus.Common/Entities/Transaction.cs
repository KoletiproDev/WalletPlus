using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlus.Common.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public string Reference { get; set; }

        public Guid CustomerId { get; set; }

        public TransactionTypeEnum Type { get; set; }

        public decimal Amount { get; set; }

        public Guid? BeneficiaryCustomerId { get; set; }

        public DateTime Date { get; set; }

        public Customer Customer { get; set; }
        public Customer BeneficiaryCustomer { get; set; }
    }
}

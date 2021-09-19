using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;

namespace WalletPlus.Common.Models
{
    
    public class WalletModel
    {
        public Guid Id { get; set; }

        public CustomerModel Customer { get; set; }

        public WalletTypeEnum Type { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

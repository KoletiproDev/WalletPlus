using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlus.Common.Entities
{
    public enum WalletTypeEnum
    {
        Transactional = 1,
        Point = 2
    }

    public enum TransactionTypeEnum
    {
        Credit = 1,
        Debit = 2
    }
}

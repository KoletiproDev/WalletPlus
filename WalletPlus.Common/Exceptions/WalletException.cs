using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlus.Common.Exceptions
{
    public class WalletException: Exception
    {
        public WalletException(string message) : base(message)
        {

        }
    }
}

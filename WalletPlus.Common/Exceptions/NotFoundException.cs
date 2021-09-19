using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletPlus.Common.Exceptions
{
    public class NotFoundException : Exception 
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}

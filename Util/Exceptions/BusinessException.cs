using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
}

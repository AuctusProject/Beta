using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util.Exceptions
{
    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}

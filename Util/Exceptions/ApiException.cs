using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public int Code { get; private set; }
        public ApiException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}

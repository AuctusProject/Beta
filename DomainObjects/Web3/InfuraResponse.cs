using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Web3
{
    public class InfuraResponse
    {
        public string Jsonrpc { get; set; }
        public int? Id { get; set; }
        public ErrorResponse Error { get; set; }
        public object Result { get; set; }

        public class ErrorResponse
        {
            public string Message { get; set; }
            public int? Code { get; set; }
        }
    }
}

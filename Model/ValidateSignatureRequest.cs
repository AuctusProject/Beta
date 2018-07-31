using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Model
{
    public class ValidateSignatureRequest
    {
        public string Address { get; set; }
        public string Signature { get; set; }
    }
}

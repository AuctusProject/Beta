using System;
using System.IO;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Nethereum.Signer;

namespace Auctus.Util
{
    public class Signature
    {
        public static string HashAndEcRecover(string plainMessage, string signature)
        {
            return new MessageSigner().HashAndEcRecover($"\u0019Ethereum Signed Message:\n{plainMessage.Length}{plainMessage}", signature);
        }
    }
}

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

namespace Auctus.Util
{
    /// <summary>
    ///     Based on the implementation of NEthereum
    ///     https://github.com/Nethereum/NEthereum
    /// </summary>

    internal static class HexByteConvertorExtensions
    {
        //From article http://blogs.msdn.com/b/heikkiri/archive/2012/07/17/hex-string-to-corresponding-byte-array.aspx

        private static readonly byte[] Empty = new byte[0];

        internal static string ToHex(this byte[] value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToString("x2")).ToArray());
        }

        private static byte[] HexToByteArrayInternal(string value)
        {
            byte[] bytes = null;
            if (string.IsNullOrEmpty(value))
            {
                bytes = Empty;
            }
            else
            {
                var string_length = value.Length;
                var character_index = value.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
                // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.               
                var number_of_characters = string_length - character_index;

                var add_leading_zero = false;
                if (0 != number_of_characters % 2)
                {
                    add_leading_zero = true;

                    number_of_characters += 1; // Leading '0' has been striped from the string presentation.
                }

                bytes = new byte[number_of_characters / 2]; // Initialize our byte array to hold the converted string.

                var write_index = 0;
                if (add_leading_zero)
                {
                    bytes[write_index++] = FromCharacterToByte(value[character_index], character_index);
                    character_index += 1;
                }

                for (var read_index = character_index; read_index < value.Length; read_index += 2)
                {
                    var upper = FromCharacterToByte(value[read_index], read_index, 4);
                    var lower = FromCharacterToByte(value[read_index + 1], read_index + 1);

                    bytes[write_index++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        internal static byte[] HexToByteArray(this string value)
        {
            try
            {
                return HexToByteArrayInternal(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(
                    "String '{0}' could not be converted to byte array (not hex?).", value), ex);
            }
        }

        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            var value = (byte)character;
            if (0x40 < value && 0x47 > value || 0x60 < value && 0x67 > value)
            {
                if (0x40 == (0x40 & value))
                    if (0x20 == (0x20 & value))
                        value = (byte)((value + 0xA - 0x61) << shift);
                    else
                        value = (byte)((value + 0xA - 0x41) << shift);
            }
            else if (0x29 < value && 0x40 > value)
            {
                value = (byte)((value - 0x30) << shift);
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));
            }

            return value;
        }
    }

    public class Signature
    {
        internal class EthECKey
        {
            private readonly ECKey _ecKey;

            internal EthECKey(ECKey ecKey)
            {
                _ecKey = ecKey;
            }

            internal string GetPublicAddress()
            {
                var initaddr = new Sha3Keccack().CalculateHash(GetPubKeyNoPrefix());
                var addr = new byte[initaddr.Length - 12];
                Array.Copy(initaddr, 12, addr, 0, initaddr.Length - 12);
                return addr.ToHex();
            }

            internal byte[] GetPubKeyNoPrefix()
            {
                var pubKey = _ecKey.GetPubKey(false);
                var arr = new byte[pubKey.Length - 1];
                //remove the prefix
                Array.Copy(pubKey, 1, arr, 0, arr.Length);
                return arr;
            }

            internal static int GetRecIdFromV(byte[] v)
            {
                return GetRecIdFromV(v[0]);
            }

            internal static int GetRecIdFromV(byte v)
            {
                var header = v;
                // The header byte: 0x1B = first key with even y, 0x1C = first key with odd y,
                //                  0x1D = second key with even y, 0x1E = second key with odd y
                if (header < 27 || header > 34)
                    throw new Exception("Header byte out of range: " + header);
                if (header >= 31)
                    header -= 4;
                return header - 27;
            }

            internal static EthECKey RecoverFromSignature(EthECDSASignature signature, byte[] hash)
            {
                return new EthECKey(ECKey.RecoverFromSignature(GetRecIdFromV(signature.V), signature.ECDSASignature, hash,
                    false));
            }
        }

        /// <summary>
        ///     ECKey based on the implementation of NBitcoin
        ///     https://github.com/MetacoSA/NBitcoin
        ///     Removed the dependency of the custom BouncyCastle
        /// </summary>
        internal class ECKey
        {
            internal static readonly X9ECParameters _Secp256k1 = SecNamedCurves.GetByName("secp256k1");
            private readonly ECKeyParameters _Key;

            private ECDomainParameters _DomainParameter;

            internal ECKey(byte[] vch, bool isPrivate)
            {
                if (isPrivate)
                {
                    _Key = new ECPrivateKeyParameters(new BigInteger(1, vch), DomainParameter);
                }
                else
                {
                    var q = Secp256k1.Curve.DecodePoint(vch);
                    _Key = new ECPublicKeyParameters("EC", q, DomainParameter);
                }
            }

            internal ECPrivateKeyParameters PrivateKey => _Key as ECPrivateKeyParameters;

            internal static X9ECParameters Secp256k1 => _Secp256k1;

            internal ECDomainParameters DomainParameter
            {
                get
                {
                    if (_DomainParameter == null)
                        _DomainParameter = new ECDomainParameters(Secp256k1.Curve, Secp256k1.G, Secp256k1.N, Secp256k1.H);
                    return _DomainParameter;
                }
            }

            internal byte[] GetPubKey(bool isCompressed)
            {
                var q = GetPublicKeyParameters().Q;
                //Pub key (q) is composed into X and Y, the compressed form only include X, which can derive Y along with 02 or 03 prepent depending on whether Y in even or odd.
                q = q.Normalize();
                var result =
                    Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(isCompressed);
                return result;
            }

            internal ECPublicKeyParameters GetPublicKeyParameters()
            {
                if (_Key is ECPublicKeyParameters)
                    return (ECPublicKeyParameters)_Key;
                var q = Secp256k1.G.Multiply(PrivateKey.D);
                return new ECPublicKeyParameters("EC", q, DomainParameter);
            }

            internal static ECKey RecoverFromSignature(int recId, ECDSASignature sig, byte[] message, bool compressed)
            {
                if (recId < 0)
                    throw new ArgumentException("recId should be positive");
                if (sig.R.SignValue < 0)
                    throw new ArgumentException("r should be positive");
                if (sig.S.SignValue < 0)
                    throw new ArgumentException("s should be positive");
                if (message == null)
                    throw new ArgumentNullException("message");


                var curve = Secp256k1;

                // 1.0 For j from 0 to h   (h == recId here and the loop is outside this function)
                //   1.1 Let x = r + jn

                var n = curve.N;
                var i = BigInteger.ValueOf((long)recId / 2);
                var x = sig.R.Add(i.Multiply(n));

                //   1.2. Convert the integer x to an octet string X of length mlen using the conversion routine
                //        specified in Section 2.3.7, where mlen = ⌈(log2 p)/8⌉ or mlen = ⌈m/8⌉.
                //   1.3. Convert the octet string (16 set binary digits)||X to an elliptic curve point R using the
                //        conversion routine specified in Section 2.3.4. If this conversion routine outputs “invalid”, then
                //        do another iteration of Step 1.
                //
                // More concisely, what these points mean is to use X as a compressed public key.

                //using bouncy and Q value of Point
                var prime = new BigInteger(1,
                    Org.BouncyCastle.Utilities.Encoders.Hex.Decode(
                        "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F"));
                if (x.CompareTo(prime) >= 0)
                    return null;

                // Compressed keys require you to know an extra bit of data about the y-coord as there are two possibilities.
                // So it's encoded in the recId.
                var R = DecompressKey(x, (recId & 1) == 1);
                //   1.4. If nR != point at infinity, then do another iteration of Step 1 (callers responsibility).

                if (!R.Multiply(n).IsInfinity)
                    return null;

                //   1.5. Compute e from M using Steps 2 and 3 of ECDSA signature verification.
                var e = new BigInteger(1, message);
                //   1.6. For k from 1 to 2 do the following.   (loop is outside this function via iterating recId)
                //   1.6.1. Compute a candidate public key as:
                //               Q = mi(r) * (sR - eG)
                //
                // Where mi(x) is the modular multiplicative inverse. We transform this into the following:
                //               Q = (mi(r) * s ** R) + (mi(r) * -e ** G)
                // Where -e is the modular additive inverse of e, that is z such that z + e = 0 (mod n). In the above equation
                // ** is point multiplication and + is point addition (the EC group operator).
                //
                // We can find the additive inverse by subtracting e from zero then taking the mod. For example the additive
                // inverse of 3 modulo 11 is 8 because 3 + 8 mod 11 = 0, and -3 mod 11 = 8.

                var eInv = BigInteger.Zero.Subtract(e).Mod(n);
                var rInv = sig.R.ModInverse(n);
                var srInv = rInv.Multiply(sig.S).Mod(n);
                var eInvrInv = rInv.Multiply(eInv).Mod(n);
                var q = ECAlgorithms.SumOfTwoMultiplies(curve.G, eInvrInv, R, srInv);
                q = q.Normalize();
                if (compressed)
                {
                    q = Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger());
                    return new ECKey(q.GetEncoded(true), false);
                }
                return new ECKey(q.GetEncoded(), false);
            }
            
            private static ECPoint DecompressKey(BigInteger xBN, bool yBit)
            {
                var curve = Secp256k1.Curve;
                var compEnc = X9IntegerConverter.IntegerToBytes(xBN, 1 + X9IntegerConverter.GetByteLength(curve));
                compEnc[0] = (byte)(yBit ? 0x03 : 0x02);
                return curve.DecodePoint(compEnc);
            }
        }

        internal class ECDSASignature
        {
            internal ECDSASignature(BigInteger r, BigInteger s)
            {
                R = r;
                S = s;
            }

            internal BigInteger R { get; }

            internal BigInteger S { get; }

            internal byte[] V { get; set; }
        }

        internal class EthECDSASignature
        {
            internal EthECDSASignature(BigInteger r, BigInteger s)
            {
                ECDSASignature = new ECDSASignature(r, s);
            }

            internal ECDSASignature ECDSASignature { get; }

            internal byte[] V
            {
                get => ECDSASignature.V;
                set => ECDSASignature.V = value;
            }
        }

        internal class EthECDSASignatureFactory
        {
            internal static EthECDSASignature FromComponents(byte[] r, byte[] s)
            {
                return new EthECDSASignature(new BigInteger(1, r), new BigInteger(1, s));
            }

            internal static EthECDSASignature FromComponents(byte[] r, byte[] s, byte v)
            {
                var signature = FromComponents(r, s);
                signature.V = new[] { v };
                return signature;
            }
        }

        internal class Sha3Keccack
        {
            internal byte[] CalculateHash(byte[] value)
            {
                var digest = new KeccakDigest(256);
                var output = new byte[digest.GetDigestSize()];
                digest.BlockUpdate(value, 0, value.Length);
                digest.DoFinal(output, 0);
                return output;
            }

            internal string CalculateHash(string value)
            {
                var input = Encoding.UTF8.GetBytes(value);
                var output = CalculateHash(input);
                return output.ToHex();
            }
        }

        public static string HashAndEcRecover(string plainMessage, string signature)
        {
            var hashMessage = new Sha3Keccack().CalculateHash(Encoding.UTF8.GetBytes(plainMessage));
            var ecdaSignature = ExtractEcdsaSignature(signature);
            return EthECKey.RecoverFromSignature(ecdaSignature, hashMessage).GetPublicAddress();
        }

        private static EthECDSASignature ExtractEcdsaSignature(string signature)
        {
            var signatureArray = signature.HexToByteArray();

            var v = signatureArray[64];

            if (v == 0 || v == 1)
                v = (byte)(v + 27);

            var r = new byte[32];
            Array.Copy(signatureArray, r, 32);
            var s = new byte[32];
            Array.Copy(signatureArray, 32, s, 0, 32);

            var ecdaSignature = EthECDSASignatureFactory.FromComponents(r, s, v);
            return ecdaSignature;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util
{
    public static class DoubleExtensions
    {
        private const double _1 = 0.1;
        private const double _2 = 0.01;
        private const double _3 = 0.001;
        private const double _4 = 0.0001;
        private const double _5 = 0.00001;
        private const double _6 = 0.000001;
        private const double _7 = 0.0000001;
        private const double _8 = 0.00000001;
        private const double _9 = 0.000000001;
        private const double _10 = 0.000000001;

        public static bool Equals1DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _1);
        }
        public static bool Equals2DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _2);
        }
        public static bool Equals3DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _3);
        }
        public static bool Equals4DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _4);
        }
        public static bool Equals5DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _5);
        }
        public static bool Equals6DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _6);
        }
        public static bool Equals7DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _7);
        }
        public static bool Equals8DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _8);
        }
        public static bool Equals9DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _9);
        }
        public static bool Equals10DigitPrecision(this double left, double right)
        {
            return EqualsDigitPrecision(left, right, _10);
        }
        private static bool EqualsDigitPrecision(double left, double right, double precision)
        {
            return Math.Abs(left - right) < precision;
        }
    }
}

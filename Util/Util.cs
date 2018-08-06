using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Threading;
using gfoidl.DataCompression;
using System.Linq;

namespace Auctus.Util
{
    public class Util
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        public static DateTime UnixMillisecondsTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;
        }

        public static Int64 DatetimeToUnixSeconds(DateTime datetime)
        {
            return (Int64)(datetime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static Int64 DatetimeToUnixMilliseconds(DateTime datetime)
        {
            return (Int64)(datetime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        public static double? ConvertMonthlyToDailyRate(double? monthlyRate)
        {
            if (!monthlyRate.HasValue)
                return null;
            return Math.Pow((monthlyRate.Value / 100.0) + 1.0, 1.0 / 30.0) - 1.0;
        }

        public static decimal ConvertBigNumber(string bigNumber, int decimals)
        {
            char separator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (bigNumber.Length <= decimals)
                return decimal.Parse($"0{separator}{bigNumber.PadLeft(decimals, '0')}");
            else
            {
                var integerPart = bigNumber.Substring(0, bigNumber.Length - decimals);
                var decimalPart = bigNumber.Substring(bigNumber.Length - decimals, decimals);
                return decimal.Parse($"{integerPart}{separator}{decimalPart}");
            }
        }

        public static decimal ConvertHexaBigNumber(string hexaNumber, int decimals)
        {
            return ConvertBigNumber(BigInteger.Parse("0" + hexaNumber.TrimStart('0', 'x'), NumberStyles.AllowHexSpecifier).ToString(), decimals);
        }

        public static Dictionary<DateTime, double> SwingingDoorCompression(Dictionary<DateTime, double> data)
        {
            var compression = data.Average(c => c.Value) * 0.025;
            var totalMinutes = (int)Math.Ceiling(data.Max(c => c.Key).Subtract(data.Min(c => c.Key)).TotalMinutes);
            return data.Select(c => new DataPoint(c.Key, c.Value)).SwingingDoorCompression(compression,
                GetMaximumTimeSpanForDataCompression(totalMinutes), null).ToDictionary(c => new DateTime(Convert.ToInt64(c.X)), c => c.Y);
        }

        private static TimeSpan GetMaximumTimeSpanForDataCompression(int totalMinutes)
        {
            var expected = totalMinutes / 300;
            if (expected > 5)
            {
                var possibilities = new int[] { 5, 10, 15, 20, 30, 45, 60, 90, 120, 180, 240, 360, 480, 720, 1440 };
                expected = possibilities.Where(c => c <= expected).OrderByDescending(c => c).First();
            }
            else
                expected = 5;
            return new TimeSpan(0, 0, expected, 0);
        }
    }
}

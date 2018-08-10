using System;
using System.Collections.Generic;
using System.Linq;

namespace Auctus.Util
{
    public static class SwingingDoorCompression
    {
        public static SortedDictionary<DateTime, double> Compress(Dictionary<DateTime, double> data, double? compressionWindow = null, int? maximumMinutesBetweenPoints = null)
        {
            if (!data.Any())
                return new SortedDictionary<DateTime, double>();

            maximumMinutesBetweenPoints = maximumMinutesBetweenPoints ?? GetMaximumMinutesForDataCompression(Math.Ceiling(data.Max(c => c.Key).Subtract(data.Min(c => c.Key)).TotalMinutes));
            var maximumTimeWindow = new TimeSpan(0, maximumMinutesBetweenPoints.Value, 0).Ticks;

            compressionWindow = compressionWindow ?? data.Average(c => c.Value) * 0.005;

            var orderedData = data.OrderBy(c => c.Key);
            var compressedData = new SortedDictionary<DateTime, double>();
            var operationData = new SortedDictionary<DateTime, double>();
            compressedData[orderedData.ElementAt(0).Key] = orderedData.ElementAt(0).Value;
            var lastProcessed = compressedData.Last();
            for (int i = 1; i < orderedData.Count(); ++i)
            {
                var currentData = orderedData.ElementAt(i);

                if (currentData.Key.Ticks - lastProcessed.Key.Ticks > maximumTimeWindow)
                {
                    compressedData[orderedData.ElementAt(i - 1).Key] = orderedData.ElementAt(i - 1).Value;
                    lastProcessed = orderedData.ElementAt(i - 1);
                    operationData = new SortedDictionary<DateTime, double>();
                }
                else if (operationData.Any())
                {
                    foreach(KeyValuePair<DateTime, double> operationalPair in operationData)
                    {
                        if (GetDistance(lastProcessed, currentData, operationalPair) > compressionWindow.Value)
                        {
                            compressedData[orderedData.ElementAt(i - 1).Key] = orderedData.ElementAt(i - 1).Value;
                            lastProcessed = orderedData.ElementAt(i - 1);
                            operationData = new SortedDictionary<DateTime, double>();
                            break;
                        }
                    }
                }
                operationData[currentData.Key] = currentData.Value;
            }
            return compressedData;
        }

        private static double GetDistance(KeyValuePair<DateTime, double> lastProcessed, KeyValuePair<DateTime, double> current, KeyValuePair<DateTime, double> operational)
        {
            var a = (current.Value - lastProcessed.Value) / (current.Key.Ticks - lastProcessed.Key.Ticks);
            var c = lastProcessed.Value - a * lastProcessed.Key.Ticks;
            return Math.Abs(a * operational.Key.Ticks - operational.Value + c) / Math.Sqrt(a * a + 1);
        }

        private static int GetMaximumMinutesForDataCompression(double totalMinutes)
        {
            var expected = totalMinutes / 500;
            if (expected > 15)
            {
                var possibilities = new int[] { 15, 20, 30, 45, 60, 90, 120, 180, 240, 360, 480, 720, 1440 };
                expected = possibilities.Where(c => c <= expected).OrderByDescending(c => c).First();
            }
            else
                expected = 15;
            return (int)expected;
        }
    }
}

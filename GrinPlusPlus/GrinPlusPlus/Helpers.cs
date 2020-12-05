using System;

namespace GrinPlusPlus
{
    public static class Helpers
    {
        public static string GetInteger(double number)
        {
            string d = (number / Math.Pow(10, 9)).ToString();
            if (d.IndexOf(".") > -1)
            {
                d = d.Substring(0, d.IndexOf("."));
            }
            return d.Replace(".", "");
        }

        public static string GetDecimals(double number)
        {
            string d = (number / Math.Pow(10, 9)).ToString();
            if (d.IndexOf(".") > -1)
            {
                d = d.Substring(d.IndexOf("."));
                return d.PadRight(9, '0').Replace(".", "");
            }
            return ".".PadRight(10, '0').Replace(".", "");
        }

        public static double GetPercentage(ulong numerator, ulong denominator)
        {
            if (numerator == 0 || denominator == 0) return 0;
            if (denominator <= 0)
            {
                return 0;
            }
            return Math.Round(100 * ((float)numerator / (float)denominator));
        }
    }
}

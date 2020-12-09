using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GrinPlusPlus
{
    public static class Helpers
    {
        public static string GetInteger(double number)
        {
            string d = (number / Math.Pow(10, 9)).ToString(new CultureInfo("en-US"));
            if (d.IndexOf(".") > -1)
            {
                d = d.Substring(0, d.IndexOf("."));
            }
            return d.Replace(".", "");
        }

        public static string GetDecimals(double number)
        {
            string d = (number / Math.Pow(10, 9)).ToString(new CultureInfo("en-US"));
            if (d.IndexOf(".") > -1)
            {
                d = d.Substring(d.IndexOf("."));
                return d.PadRight(9, '0').Replace(".", "");
            }
            return ".".PadRight(10, '0').Replace(".", "");
        }

        public static double GetFraction(ulong numerator, ulong denominator)
        {
            if (numerator == 0 || denominator == 0) return 0;
            if (denominator <= 0)
            {
                return 0;
            }
            return numerator / (float)denominator;
        }

        public static async Task<string> MakeTcpRequestAsync(string message, TcpClient client, bool readToEnd = true)
        {
            client.ReceiveTimeout = 20000;
            client.SendTimeout = 20000;
            string proxyResponse = string.Empty;

            try
            {
                // Send message
                using (StreamWriter streamWriter = new StreamWriter(client.GetStream()))
                {
                    streamWriter.Write(message);
                    streamWriter.Flush();
                }

                // Read response
                using (StreamReader streamReader = new StreamReader(client.GetStream()))
                {
                    proxyResponse = readToEnd ? await streamReader.ReadToEndAsync() : await streamReader.ReadLineAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return proxyResponse;
        }

        public static double TextCurrencyToDouble(string text)
        {
            var t = text;
            var dot_pos = t.LastIndexOf(".");
            var comma_pos = t.LastIndexOf(",");
            if (comma_pos > dot_pos)
            {
                t = t.Replace(".", "");
                t = t.Replace(",", ".");
            }
            else
            {
                t = t.Replace(",", "");
            }
            return Double.Parse(t, new CultureInfo("en-US"));
        }
    }
}

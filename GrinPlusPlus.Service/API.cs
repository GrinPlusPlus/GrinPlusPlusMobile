using System;
using System.Net.Http;

namespace GrinPlusPlus.Service
{
    internal class API
    {
        public static readonly HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(5) };
    }
}
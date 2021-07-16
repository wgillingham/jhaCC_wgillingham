using System;

namespace jhaCC.Utility
{
    static class URLHelpers
    {
        public static string GetDomainNameOfUrlString(string urlString)
        {
            var host = new Uri(urlString).Host;
            return host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
        }
    }
}

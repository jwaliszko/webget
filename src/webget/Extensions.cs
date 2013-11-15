using System;
using System.IO;
using System.Linq;
using System.Net;

namespace webget
{
    internal static class Extensions
    {
        public static WebClient Proxify(this WebClient client, ProxySettings data)
        {
            var proxy = data.Port.HasValue ? new WebProxy(data.Host, data.Port.Value) : new WebProxy(data.Host);
            if (data.AuthRequired)
                proxy.Credentials = new NetworkCredential(data.AuthUsername, data.AuthPassword);
            client.Proxy = proxy;
            return client;
        }

        public static string ToAbsoluteUri(this string relativeUri, string baseUri)
        {
            if (!Uri.IsWellFormedUriString(relativeUri, UriKind.RelativeOrAbsolute) ||
                !Uri.IsWellFormedUriString(baseUri, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("invalid uri format");

            Uri relative;
            return !Uri.TryCreate(relativeUri, UriKind.Relative, out relative)
                       ? relativeUri    // relative cannot be created - it is absolute, return it
                       : new Uri(new Uri(baseUri), relative).ToString();
        }

        public static bool EndsWithAny(this string path, string[] extensions)
        {
            var extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) &&
                   extensions.Contains(extension.Replace(".", ""),
                                       StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(this string[] target, string value, StringComparison comparison)
        {
            return target.ToList().FindAll(x => x.Equals(value, comparison)).Count > 0;
        }
    }
}

using System;
using System.Net;

namespace webget
{
    internal class NetClient: WebClient
    {
        public string UserAgent { get; set; }
        public ProxySettings ProxyData { get; set; }
        public bool HeadOnly { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if (ProxyData != null)
                Proxify(this, ProxyData);
            if (!string.IsNullOrEmpty(UserAgent))
                Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            var req = base.GetWebRequest(address);
            if (HeadOnly && req != null && req.Method == "GET")
            {
                req.Method = "HEAD";
            }

            return req;
        }

        private void Proxify(WebClient client, ProxySettings data)
        {
            var proxy = data.Port.HasValue ? new WebProxy(data.Host, data.Port.Value) : new WebProxy(data.Host);
            if (data.AuthRequired)
                proxy.Credentials = new NetworkCredential(data.AuthUsername, data.AuthPassword);
            client.Proxy = proxy;
        }
    }
}

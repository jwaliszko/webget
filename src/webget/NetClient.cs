using System;
using System.Net;

namespace webget
{
    internal class NetClient: WebClient
    {
        public string UserAgent { get; set; }
        public ProxySettings ProxyData { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if(ProxyData!= null)
                this.Proxify(ProxyData);
            if(!string.IsNullOrEmpty(UserAgent))
                Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
            Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");            
            return base.GetWebRequest(address);
        }
    }
}

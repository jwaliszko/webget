namespace webget
{
    internal class ProxySettings
    {
        public string Host { get; set; }
        public int? Port { get; set; }
        public bool AuthRequired { get; set; }
        public string AuthUsername { get; set; }
        public string AuthPassword { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace webget
{
    internal class Getter
    {
        private readonly IList<string> _visitedUris = new List<string>();

        public ProxySettings ProxyData { get; set; }
        public string UserAgent { get; set; }
        public string RecursionTarget { get; set; }
        public string Uri { get; set; }
        public string SaveDirectory { get; set; }
        public string NameFilter { get; set; }
        public string[] Extensions { get; set; }
        public int GreaterThan { get; set; }
        public int LessThan { get; set; }
        public int RecursionDepth { get; set; }
        public bool LinkLabel { get; set; }        

        public void Execute()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            using (var client = new NetClient {ProxyData = ProxyData, Encoding = Encoding.UTF8, UserAgent = UserAgent})
            {
                ExecuteInternal(client, Uri, Extensions, SaveDirectory, RecursionDepth);
            }
        }

        private void ExecuteInternal(NetClient client, string uri, string[] extensions, string directory,
                                     int maxDepth, int currentDepth = 0)
        {
            _visitedUris.Add(uri);
            var content = WebUtility.HtmlDecode(DownloadString(client, uri, currentDepth));
            if (!string.IsNullOrEmpty(content))
            {
                var resources = ContentParser.ExtractUris(content)
                                             .Where(x => x.Value.EndsWithAny(extensions))
                                             .Select(x => new UriData
                                                 {
                                                     Value = x.Value.ToAbsoluteUri(uri),
                                                     Label = x.Label.Normailze(100)
                                                 })
                                             .Distinct(x => x.Value);
                DownloadFiles(client, resources, directory, currentDepth);

                if (maxDepth < 0 || currentDepth < maxDepth)
                {
                    var sites = ContentParser.ExtractUris(content)
                                             .Where(x => x.Value.WithoutExtension())
                                             .Select(x => x.Value.ToAbsoluteUri(uri))
                                             .Where(x => !_visitedUris.Contains(x, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(RecursionTarget))
                        sites = sites.Where(x => (new Regex(RecursionTarget, RegexOptions.IgnoreCase)).IsMatch(x));

                    currentDepth++;
                    foreach (var s in sites)
                    {                        
                        ExecuteInternal(client, s, extensions, directory, maxDepth, currentDepth);
                    }
                }
            }
        }

        private string DownloadString(NetClient client, string uri, int currentDepth)
        {
            try
            {
                Console.WriteLine(@"[--> {0}]: ""{1}""...", currentDepth, uri);
                return client.DownloadString(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                return null;
            }
        }

        private void DownloadFiles(NetClient client, IEnumerable<UriData> uris, string directory, int currentDepth)
        {
            var i = -1;
            foreach (var uri in uris)
            {
                var name = LinkLabel && !string.IsNullOrEmpty(uri.Label)
                               ? string.Format("{0}.{1}", uri.Label, uri.Value.Split('.').Last())
                               : uri.Value.Split('/').Last();

                if (!string.IsNullOrEmpty(NameFilter) &&
                    !name.Contains(NameFilter, StringComparison.OrdinalIgnoreCase))
                    continue;

                var path = Path.Combine(directory, name);
                if (File.Exists(path))
                {
                    Console.WriteLine(@"""{0}"" already exists, skipping...", name);
                    continue;
                }

                try
                {
                    if (GreaterThan > 0 || LessThan > 0)
                    {
                        var size = DownloadHeader(client, uri.Value, "Content-Length");
                        int contentLength;
                        if (int.TryParse(size, out contentLength))
                        {
                            if (GreaterThan > 0 && contentLength < GreaterThan)
                                continue;
                            if (LessThan > 0 && contentLength > LessThan)
                                continue;
                        }
                    }
                    Console.WriteLine(@"[{0}.{1}]: downloading ""{2}""...", currentDepth, ++i, name);
                    client.DownloadFile(uri.Value, path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: {0}", ex.Message);
                }
            }
        }

        private string DownloadHeader(NetClient client, string uri, string header)
        {
            client.HeadOnly = true;
            client.DownloadData(uri);
            client.HeadOnly = false;
            return client.ResponseHeaders.Get(header);           
        }
    }
}
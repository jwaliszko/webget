using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace webget
{
    internal class Getter
    {
        public ProxySettings ProxyData { get; set; }
        public string UserAgent { get; set; }
        
        public void Execute(string uri, string[] extensions, string directory, int maxDepth)
        {
            using (var client = new NetClient {ProxyData = ProxyData, Encoding = Encoding.UTF8, UserAgent = UserAgent})
            {
                ExecuteInternal(client, uri, extensions, directory, maxDepth);
            }
        }

        private void ExecuteInternal(WebClient client, string uri, string[] extensions, string directory,
                                     int maxDepth, int currentDepth = 0)
        {
            var content = DownloadString(client, uri, currentDepth);
            if (!string.IsNullOrEmpty(content))
            {                
                var resources = ContentParser.ExtractUris(content)
                                             .Where(x => x.EndsWithAny(extensions))
                                             .Select(x => x.ToAbsoluteUri(uri)).ToArray();
                var currentDirectory = PrepareDirectory(directory, currentDepth);
                DownloadFiles(client, resources, currentDirectory);

                if (maxDepth < 0 || currentDepth < maxDepth)
                {
                    var sites = ContentParser.ExtractUris(content)
                                             .Where(x => x.WithoutExtension())
                                             .Select(x => x.ToAbsoluteUri(uri)).ToArray();
                    currentDepth++;
                    foreach (var s in sites)
                    {
                        ExecuteInternal(client, s, extensions, directory, maxDepth, currentDepth);
                    }
                }
            }
        }

        private static string PrepareDirectory(string directory, int currentDepth)
        {
            var currentDirectory = currentDepth == 0
                                       ? directory
                                       : Path.Combine(directory, string.Format("depth_{0}", currentDepth));
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);
            return currentDirectory;
        }

        private string DownloadString(WebClient client, string uri, int currentDepth)
        {
            try
            {
                Console.WriteLine(@"[depth {0}]: ""{1}""...", currentDepth, uri);
                return client.DownloadString(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                return null;
            }
        }

        private void DownloadFiles(WebClient client, string[] uris, string directory)
        {
            var i = -1;
            foreach (var uri in uris)
            {
                i++;
                var name = uri.Split('/').Last();                
                var path = Path.Combine(directory, name);
                if (File.Exists(path))
                {
                    Console.WriteLine(@"[{0}]: ""{1}"" already exists, skipping...", i, name);
                    continue;
                }

                try
                {
                    Console.WriteLine(@"[{0}]: downloading ""{1}""...", i, name);
                    client.DownloadFile(uri, path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: {0}", ex.Message);
                }
            }
        }
    }
}
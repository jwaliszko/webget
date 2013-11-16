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
        
        public void Execute(string source, string[] extensions, string directory, int maxDepth)
        {
            using (var client = new NetClient {ProxyData = ProxyData, Encoding = Encoding.UTF8, UserAgent = UserAgent})
            {
                ExecuteInternal(client, source, extensions, directory, maxDepth);
            }
        }

        private void ExecuteInternal(WebClient client, string source, string[] extensions, string directory,
                                     int maxDepth, int currentDepth = 0)
        {
            var content = client.DownloadString(source);
            var resources = ContentParser.ExtractUris(content)
                                         .Where(x => x.EndsWithAny(extensions))
                                         .Select(x => x.ToAbsoluteUri(source)).ToArray();

            var currentDirectory = currentDepth == 0
                                       ? directory
                                       : Path.Combine(directory, string.Format("depth_{0}", currentDepth));
            if (!Directory.Exists(currentDirectory))
                Directory.CreateDirectory(currentDirectory);

            Console.WriteLine(@"[depth {0}]: ""{1}""...", currentDepth, source);
            Download(client, resources, currentDirectory);

            if (maxDepth < 0 || currentDepth < maxDepth)
            {
                var sites = ContentParser.ExtractUris(content)
                                         .Where(x => x.WithoutExtension())
                                         .Select(x => x.ToAbsoluteUri(source)).ToArray();
                ++currentDepth;
                foreach (var uri in sites)
                {
                    ExecuteInternal(client, uri, extensions, directory, maxDepth, currentDepth);
                }
            }
        }

        private void Download(WebClient client, string[] uris, string directory)
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
                catch (Exception e)
                {
                    Console.WriteLine("error: {0}", e.Message);
                }
            }
        }
    }
}
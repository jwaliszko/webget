using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace webget
{
    internal class Getter
    {
        public ProxySettings ProxyData { get; set; }
        public string UserAgent { get; set; }

        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public void Execute(string source, string[] extensions, string directory)
        {
            using (var client = new NetClient {ProxyData = ProxyData, Encoding = Encoding.UTF8, UserAgent = UserAgent})
            {
                var content = client.DownloadString(source);
                var uris =
                    ContentParser.ExtractUris(content)
                                 .Where(x => x.EndsWithAny(extensions))
                                 .Select(x => x.ToAbsoluteUri(source));

                var i = -1;
                foreach (var uri in uris)
                {
                    i++;
                    var name = uri.Split('/').Last();

                    directory = directory ?? AssemblyDirectory;
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    var path = Path.Combine(directory, name);

                    if (File.Exists(path))
                    {
                        Console.WriteLine("[{0}]: {1} already exists, skipping...", i, name);
                        continue;
                    }

                    try
                    {
                        Console.WriteLine("[{0}]: downloading {1}...", i, name);
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
}
using System;
using System.IO;
using System.Reflection;

namespace webget
{
    class Program
    {
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

        private static void Main(string[] args)
        {
            try
            {
                var arg = new Argument();
                arg.Build(args);
                if (arg.Help)
                {
                    PrintHelp();
                    return;
                }
                
                var getter = new Getter {ProxyData = arg.ProxyData, UserAgent = arg.UserAgent};
                getter.Execute(arg.Url, arg.Extensions, arg.SaveDirectory ?? AssemblyDirectory, arg.RecursionDepth);
            }
            catch (ApplicationException ex)
            {
#if DEBUG
                PrintUsageError(ex.ToString());
#else
                PrintUsageError(ex.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                PrintError(ex.ToString());
#else
                PrintError(ex.Message);
#endif
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"webget: get files from web");
            Console.WriteLine(@"usage: webget [options]... [url]...");
            Console.WriteLine();
            Console.WriteLine(@"example: webget -e mp3,aac http://www.astronomycast.com/archive/");
            Console.WriteLine();
            Console.WriteLine(@"options: ");
            Console.WriteLine(@"  -e, --extensions=LIST                         comma-separated list of accepted extensions");
            Console.WriteLine(@"  -p, --proxy-settings=(user:pass@)ip(:port)    proxy settings (if required)");
            Console.WriteLine(@"  -s, --save-directory=PATH                     download directory");
            Console.WriteLine(@"  -r, --recursion-depth=LEVEL                   max recursion depth - default: 0, infinity: -1");
            Console.WriteLine(@"  -u, --user-agent=NAME                         User-Agent HTTP field spoof value,");
            Console.WriteLine(@"                                                e.g. ""Links (0.96; Linux 2.4.20-18.7 i586)""");
            Console.WriteLine(@"  -h, --help                                    print this help");
            Console.WriteLine();
            Console.WriteLine(@"mail bug reports and suggestions to <jaroslaw.waliszko@gmail.com>");
        }        

        private static void PrintUsageError(string message)
        {
            Console.WriteLine("webget: " + message);
            Console.WriteLine("try `webget --help` for more options");
        }        

        private static void PrintError(string message)
        {
            Console.WriteLine("error: " + message);
        }
    }
}

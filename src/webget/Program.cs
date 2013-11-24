using System;
using System.IO;
using System.Reflection;

namespace webget
{
    class Program
    {
        private static string AssemblyDirectory
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

                var getter = new Getter
                    {
                        Uri = arg.Url,
                        Extensions = arg.Extensions,
                        SaveDirectory = arg.SaveDirectory ?? AssemblyDirectory,
                        RecursionDepth = arg.RecursionDepth,
                        ProxyData = arg.ProxyData,
                        UserAgent = arg.UserAgent,
                        GreaterThan = arg.GreaterThan,
                        LessThan = arg.LessThan,
                        RecursionTarget = arg.RecursionTarget,
                        LinkLabel = arg.LinkLabel
                    };
                getter.Execute();
            }
            catch (ApplicationException ex)
            {
                PrintUsageError(ex.Message);
            }
            catch (Exception ex)
            {
                PrintError(ex.Message);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"webget: get files from web");
            Console.WriteLine(@"usage: webget [options]... [url]...");
            Console.WriteLine();
            Console.WriteLine(@"examples:");
            Console.WriteLine(@"  webget -e mp3,aac http://www.astronomycast.com/archive/");
            Console.WriteLine(@"  webget -e jpg,png http://www.reddit.com/r/space -r 10 -d space -gt 1mb -lt 20mb -t /space/\?count.*after -l");
            Console.WriteLine();
            Console.WriteLine(@"options: ");
            Console.WriteLine(@"  -e,  --extensions=LIST                         comma-separated list of accepted extensions");
            Console.WriteLine(@"  -p,  --proxy-settings=(user:pass@)ip(:port)    proxy settings (if required)");
            Console.WriteLine(@"  -d,  --save-directory=PATH                     download directory");
            Console.WriteLine(@"  -r,  --recursion-depth=NUMBER                  max recursion depth - default: 0, infinity: -1");
            Console.WriteLine(@"  -t,  --recursion-target=PATTERN                regex url pattern to direct recursive search");
            Console.WriteLine(@"  -l,  --link-label                              replace resource name with link label if possible");
            Console.WriteLine(@"  -u,  --user-agent=NAME                         User-Agent HTTP field spoof value,");
            Console.WriteLine(@"                                                 e.g. ""Links (0.96; Linux 2.4.20-18.7 i586)""");
            Console.WriteLine(@"  -gt, --greater-than=NUMBER[kb|mb]              upper file size boundary, no extent means bytes");
            Console.WriteLine(@"  -lt, --less-than=NUMBER[kb|mb]                 lower file size boundary, no extent means bytes");
            Console.WriteLine(@"  -h,  --help                                    print this help");
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

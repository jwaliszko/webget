using System;

namespace webget
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var arg = new Argument(args);                
                if (arg.Help)
                {
                    PrintHelp();
                    return;
                }
                
                var getter = new Getter {ProxyData = arg.ProxyData, UserAgent = arg.UserAgent};
                getter.Execute(arg.Source, arg.Extensions, arg.SaveDirectory);
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
            Console.WriteLine(@"example: webget -e mp3,ogg http://www.astronomycast.com/archive/");
            Console.WriteLine();
            Console.WriteLine(@"options: ");
            Console.WriteLine(@"  -e, --extensions=LIST                comma-separated list of accepted extensions");
            Console.WriteLine(@"  -p, --proxy=[user:pass@]ip[:port]    proxy settings if required");
            Console.WriteLine(@"  -u, --user-agent=NAME                value for User-Agent HTTP field, e.g. ""Links (0.96; Linux 2.4.20-18.7 i586)""");
            Console.WriteLine(@"  -s, --save-directory=PATH            output directory path for downloaded files");
            Console.WriteLine(@"  -h, --help                           print this help");
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
            Console.WriteLine("webget: " + message);
        }
    }
}

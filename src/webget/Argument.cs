using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace webget
{
    internal class Argument
    {
        public Argument()
        {
            Help = false;
            Extensions = new string[0];
        }

        public string Source { get; private set; }
        public string UserAgent { get; private set; }
        public string SaveDirectory { get; private set; }
        public bool Help { get; private set; }
        public string[] Extensions { get; private set; }
        public ProxySettings ProxyData { get; private set; }

        public Argument(IEnumerable<string> args)
            : this()
        {
            var argStack = new Queue<string>(args);
            while (argStack.Any())
            {
                var arg = argStack.Dequeue();
                switch (arg.ToLower())
                {
                    case "-h":
                    case "--help":
                        Help = true;
                        break;
                    case "-e":
                    case "--extensions":
                        if (!argStack.Any())
                            throw new ApplicationException("accepted extensions expected");
                        Extensions = argStack.Dequeue().Split(',');
                        break;
                    case "-p":
                    case "--proxy":
                        if (!argStack.Any())
                            throw new ApplicationException("proxy setting expected");
                        var r = new Regex(@"((?<user>\w+):(?<pass>\w+)@)?(?<ip>(\d{1,3}\.){3}\d{1,3})(:(?<port>\d{1,5}))?");                        
                        /* explanation of (\w+:\w+@)?(\d{1,3}\.){3}\d{1,3}(:\d{1,5})? (above version is exactly the same but with group names)
                         * (http://(\w+:\w+@)?)? - optional group of user:pass@
                         * (\d{1,3}\.){3} - three groups of one to three digits followed by a dot
                         * \d{1,3} - one to three digits
                         * (:\d{1,5})? - optional group of colon followed by one to five digits
                         */
                        var match = r.Match(argStack.Dequeue());
                        if(!match.Success)
                            throw new ApplicationException("invalid format for proxy settings, try `[user:pass@]host[:port]`");
                        ProxyData = new ProxySettings
                            {
                                Host = match.Groups["ip"].Value,
                                Port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value) : (int?) null,
                                AuthUsername = match.Groups["user"].Value,
                                AuthPassword = match.Groups["pass"].Value,
                                AuthRequired = match.Groups["user"].Success && match.Groups["pass"].Success
                            };
                        break;
                    case "-u":
                    case "--user-agent":
                        if (!argStack.Any())
                            throw new ApplicationException("puser agent identifier expected");
                        UserAgent = argStack.Dequeue();
                        break;
                    case "-s":
                    case "--save-directory":
                        if (!argStack.Any())
                            throw new ApplicationException("output directory path not provided");
                        SaveDirectory = argStack.Dequeue();
                        break;
                    default:
                        if (Source != null)
                            throw new ApplicationException("unexpected argument: " + arg);
                        Source = arg;
                        break;
                }
            }
            if (Help)
                return;
            if (Source == null)
                throw new ApplicationException("url argument missing");
            if (!Extensions.Any())
                throw new ApplicationException("accepted extensions argument missing ");
        }
    }
}

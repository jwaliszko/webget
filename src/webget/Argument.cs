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

        public string Url { get; private set; }
        public string UserAgent { get; private set; }
        public string SaveDirectory { get; private set; }
        public int RecursionDepth { get; private set; }
        public bool Help { get; private set; }
        public string[] Extensions { get; private set; }
        public ProxySettings ProxyData { get; private set; }

        public void Build(IEnumerable<string> args)
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
                            throw new ApplicationException(ValueExpected(arg));
                        Extensions = argStack.Dequeue().Split(',');
                        break;
                    case "-p":
                    case "--proxy-settings":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        var regex = new Regex(@"((?<user>\w+):(?<pass>\w+)@)?(?<ip>(\d{1,3}\.){3}\d{1,3})(:(?<port>\d{1,5}))?");                        
                        /* explanation of (\w+:\w+@)?(\d{1,3}\.){3}\d{1,3}(:\d{1,5})? (above version is exactly the same but with group names)
                         * (http://(\w+:\w+@)?)? - optional group of user:pass@
                         * (\d{1,3}\.){3} - three groups of one to three digits followed by a dot
                         * \d{1,3} - one to three digits
                         * (:\d{1,5})? - optional group of colon followed by one to five digits
                         */
                        var match = regex.Match(argStack.Dequeue());
                        if(!match.Success)
                            throw new ApplicationException(ValueInvalid(arg, ", try `[user:pass@]host[:port]`"));
                        ProxyData = new ProxySettings
                            {
                                Host = match.Groups["ip"].Value,
                                Port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value) : (int?) null,
                                AuthUsername = match.Groups["user"].Value,
                                AuthPassword = match.Groups["pass"].Value,
                                AuthRequired = match.Groups["user"].Success && match.Groups["pass"].Success
                            };
                        break;
                    case "-s":
                    case "--save-directory":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        SaveDirectory = argStack.Dequeue();
                        break;
                    case "-r":
                    case "--recursion-depth":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        int level;
                        var result = int.TryParse(argStack.Dequeue(), out level);
                        if(!result || level < -1)
                            throw new ApplicationException(ValueInvalid(arg));
                        RecursionDepth = level;
                        break;
                    case "-u":
                    case "--user-agent":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        UserAgent = argStack.Dequeue();
                        break;
                    default:
                        if (Url != null)
                            throw new ApplicationException("unexpected argument: " + arg);
                        if (!Uri.IsWellFormedUriString(arg, UriKind.RelativeOrAbsolute))
                            throw new ApplicationException(ValueInvalid("url"));
                        Url = arg;
                        break;
                }
            }
            if (Help)
                return;
            if (Url == null)
                throw new ApplicationException(ArgumentMissing("url"));
            if (!Extensions.Any())
                throw new ApplicationException(ArgumentMissing("extensions list"));
        }

        private string ValueExpected(string argument)
        {
            return string.Format("value for option `{0}` expected", argument);
        }

        private string ValueInvalid(string argument, string advice = null)
        {
            return string.Format("value for option `{0}` invalid{1}", argument, advice);
        }

        private string ArgumentMissing(string argument)
        {
            return string.Format("`{0}` argument missing", argument);
        }
    }
}

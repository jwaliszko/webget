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
        public string RecursionTarget { get; private set; }
        public string NameFilter { get; private set; }
        public int RecursionDepth { get; private set; }
        public int GreaterThan { get; private set; }
        public int LessThan { get; private set; }
        public int RequestTimeout { get; private set; } 
        public bool Help { get; private set; }
        public bool LinkLabel { get; private set; }
        public string[] Extensions { get; private set; }
        public ProxySettings ProxyData { get; private set; }               

        public void Build(IEnumerable<string> args)
        {
            var argStack = new Queue<string>(args);
            while (argStack.Any())
            {
                var arg = argStack.Dequeue();
                switch (arg.ToLowerInvariant())
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
                                Port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value) : (int?)null,
                                AuthUsername = match.Groups["user"].Value,
                                AuthPassword = match.Groups["pass"].Value,
                                AuthRequired = match.Groups["user"].Success && match.Groups["pass"].Success
                            };
                        break;
                    case "-d":
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
                        if(!int.TryParse(argStack.Dequeue(), out level) || level < -1)
                            throw new ApplicationException(ValueInvalid(arg));
                        RecursionDepth = level;
                        break;
                    case "-u":
                    case "--user-agent":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        UserAgent = argStack.Dequeue();
                        break;
                    case "-gt":
                    case "--greater-than":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        GreaterThan = GetSize(argStack.Dequeue());
                        break;
                    case "-lt":
                    case "--less-than":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        LessThan = GetSize(argStack.Dequeue());
                        break;
                    case "-t":
                    case "--recursion-target":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        RecursionTarget = argStack.Dequeue();
                        break;
                    case "-l":
                    case "--link-label":
                        LinkLabel = true;
                        break;
                    case "-n":
                    case "--name-filter":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        NameFilter = argStack.Dequeue();
                        break;
                    case "-o":
                    case "--request-timeout":
                        if (!argStack.Any())
                            throw new ApplicationException(ValueExpected(arg));
                        int timeout;
                        if (!int.TryParse(argStack.Dequeue(), out timeout) || timeout < -1)
                            throw new ApplicationException(ValueInvalid(arg));
                        RequestTimeout = timeout;
                        break;
                    default:
                        if (Url != null)
                            throw new ApplicationException("unexpected argument: " + arg);
                        if (!Uri.IsWellFormedUriString(arg, UriKind.Absolute))
                            throw new ApplicationException(ValueInvalid("url", ", remember to provide absolute url"));
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

        private int GetSize(string arg)
        {
            var regex = new Regex(@"(?<size>\d+)(?<extent>kb|mb)?");
            var match = regex.Match(arg);
            if (!match.Success)
                throw new ApplicationException(ValueInvalid(arg, ", try number with optional size extent"));
            var size = int.Parse(match.Groups["size"].Value);
            switch (match.Groups["extent"].Value)
            {
                case "kb":
                    size *= 1000;
                    break;
                case "mb":
                    size *= 1000000;
                    break;
            }
            return size;
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

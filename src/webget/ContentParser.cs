using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace webget
{
    internal static class ContentParser
    {
        public const string FromLink = @"href=""(?<uri>.*?)""";
        public const string FromSrc = @"src=""(?<uri>.*?)""";

        public static string[] ExtractUris(string html)
        {
            var regexLink = new Regex(FromLink, RegexOptions.IgnoreCase);
            var regexSrc = new Regex(FromSrc, RegexOptions.IgnoreCase);
            var uris =
                regexLink.Matches(html)
                         .Cast<Match>()
                         .Where(x => x.Groups["uri"].Success)
                         .Select(x => x.Groups["uri"].Value)
                         .Union(regexSrc.Matches(html)
                                        .Cast<Match>()
                                        .Where(x => x.Groups["uri"].Success)
                                        .Select(x => x.Groups["uri"].Value));
            return uris.Where(x => !string.IsNullOrEmpty(x) && Uri.IsWellFormedUriString(x, UriKind.RelativeOrAbsolute))
                       .Distinct()
                       .ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace webget
{
    internal static class ContentParser
    {
        public const string Href = @"href=""(?<uri>.*?)"".*?>(?<label>.*?)<";
        public const string Src = @"src=""(?<uri>.*?)"".*?(alt=""(?<label>.*?)"")?";

        public static IEnumerable<UriData> ExtractUris(string html)
        {
            var regexHref = new Regex(Href, RegexOptions.IgnoreCase);
            var regexSrc = new Regex(Src, RegexOptions.IgnoreCase);
            var uris =
                regexHref.Matches(html)
                         .Cast<Match>()
                         .Where(x => x.Groups["uri"].Success)
                         .Select(x => new {Uri = x.Groups["uri"].Value, Label = x.Groups["label"].Value})
                         .Union(regexSrc.Matches(html)
                                        .Cast<Match>()
                                        .Where(x => x.Groups["uri"].Success)
                                        .Select(x => new {Uri = x.Groups["uri"].Value, Label = x.Groups["label"].Value}));
            return uris.Where(
                x => !string.IsNullOrEmpty(x.Uri) && Uri.IsWellFormedUriString(x.Uri, UriKind.RelativeOrAbsolute))
                       .OrderBy(x => x.Uri).ThenByDescending(x => x.Label)  //important for distinct to cut off duplicates without labels first
                       .Distinct(x => x.Uri)
                       .Select(x => new UriData {Value = x.Uri, Label = x.Label});
        }
    }
}

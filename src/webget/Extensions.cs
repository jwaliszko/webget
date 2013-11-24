using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace webget
{
    internal static class Extensions
    {
        public static string ToAbsoluteUri(this string relativeUri, string baseUri)
        {
            if (!Uri.IsWellFormedUriString(relativeUri, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("invalid uri format", "relativeUri");
            if(!Uri.IsWellFormedUriString(baseUri, UriKind.RelativeOrAbsolute))
                throw new ArgumentException("invalid uri format", "baseUri");

            Uri relative;
            return !Uri.TryCreate(relativeUri, UriKind.Relative, out relative)
                       ? relativeUri // relative cannot be created - it is absolute, return it
                       : new Uri(new Uri(baseUri), relative).ToString();
        }

        public static bool EndsWithAny(this string path, string[] extensions)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            var extension = Path.GetExtension(path);
            return !string.IsNullOrEmpty(extension) &&
                   extensions.Contains(extension.Replace(".", ""),
                                       StringComparison.OrdinalIgnoreCase);
        }

        public static bool WithoutExtension(this string path)
        {
            return string.IsNullOrEmpty(Path.GetExtension(path));
        }

        public static bool Contains(this IEnumerable<string> source, string value, StringComparison comparison)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (value == null)
                throw new ArgumentNullException("value");

            return source.ToList().FindAll(x => x.Equals(value, comparison)).Count > 0;
        }

        public static bool Contains(this string source, string value, StringComparison comp)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (value == null)
                throw new ArgumentNullException("value");

            return source.IndexOf(value, comp) >= 0;
        }

        public static string Normailze(this string input, int length)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (length < 0)
                throw new ArgumentException("lenght cannot be negative", "length");

            var output = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            return output.Length > length ? string.Format("{0}...", output.Substring(0, length)) : output;
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            return source.Distinct(keySelector, EqualityComparer<TKey>.Default);
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return DistinctImpl(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> DistinctImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}

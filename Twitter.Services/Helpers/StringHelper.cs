using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text.RegularExpressions;

namespace TwitterServices.Helpers
{
    public static class StringHelper
    {
        public static decimal GetPercentage(List<string> list, int total)
        {
            if (!list.Any() || total == 0)
                return 0;

            return Math.Round((decimal)list.Count() / total * 100, 2);
        }

        public static string GetTop(List<string> list)
        {
            if (!list.Any())
                return null;

            return list.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        }

        public static List<string> ParseEmojis(string value)
        {
            var allEmojis = @"\b(\\u20a0|\\u32ff|\\ud83c|\\udc00-|\\ud83d|\\udeff|\\udbb9|\\udce5-|\\udeff|\\udbb9|\\udcee)\b";
            //didnt have time to get the full list
            return ParseString(value, allEmojis);
        }

        public static bool IsPhoto(string value)
        {
            var photoExtensions = @"\b(/photo/)\b";
            return ParseString(value, photoExtensions).Any();
        }

        public static List<string> ParseString(string source, string pattern)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(source))
                return result;

            source = source.ToLower();

            foreach (Match match in Regex.Matches(source, pattern))
            {
                if (match.Success && match.Groups.Count > 0)
                {
                    var text = match.Groups[1].Value;
                    result.Add(text);
                }
            }

            return result;
        }
    }
}

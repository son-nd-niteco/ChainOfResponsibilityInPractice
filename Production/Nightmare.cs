using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Production
{
    public static class Nightmare
    {
        private static readonly string dayPattern = "^(\\d+)d$";
        private static readonly string hourPattern = "^(\\d+)h$";
        private static readonly string minutePattern = "^(\\d+)m$";

        public static int Parse(string input)
        {
            var hasDuplication = input.Split(' ').Select(x => x.Last()).GroupBy(x => x).Any(g => g.Count() > 1);
            if (hasDuplication)
            {
                throw new Exception("unit duplicated");
            }

            return input.Split(' ').Select(x => ParseUnit(x)).Sum();
        }

        private static int ParseUnit(string input)
        {
            if (Regex.IsMatch(input, dayPattern))
            {
                return ParseDay(input);
            }

            if (Regex.IsMatch(input, hourPattern))
            {
                return ParseHour(input);
            }

            if (Regex.IsMatch(input, minutePattern))
            {
                return ParseMinute(input);
            }

            if (int.TryParse(input, out var hours))
            {
                return hours * 60;
            }

            throw new NotSupportedException("Hey, how to parse this, dude?");
        }

        private static int ParseDay(string input) =>
            int.Parse(Regex.Match(input, dayPattern).Groups[1].Value) * 60 * 24;

        private static int ParseHour(string input) =>
            int.Parse(Regex.Match(input, hourPattern).Groups[1].Value) * 60;

        private static int ParseMinute(string input) =>
            int.Parse(Regex.Match(input, minutePattern).Groups[1].Value);
    }
}
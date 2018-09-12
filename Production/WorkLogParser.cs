using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ResponsibilityChain;

namespace Production
{
    public class NitecoParser : Handler<string, int>, IWorkLogParser
    {
        public NitecoParser(Action<string> collector)
        {
            AddHandler(new IAmSpy(collector));
            AddHandler(new NoUnitDuplicationRule());
            AddHandler(new IAmSpy(collector));
            AddHandler(new SAParser(collector));
        }
    }

    public class IAmSpy : IHandler<string, int>
    {
        private readonly Action<string> _collector;

        public IAmSpy(Action<string> collector) => _collector = collector;

        public int Handle(string input, Func<string, int> next)
        {
            var victim = GetVictimName(next);

            _collector($"spying {victim}: input = {input}");
            int result;

            try
            {
                result = next(input);
            }
            catch (Exception exception)
            {
                _collector($"error: {exception.Message}");
                throw;
            }

            _collector($"spying {victim} > {result}");
            return result;
        }

        private static string GetVictimName(Func<string, int> next)
        {
            Func<object, object> getCapturedChainedDelegateClonedAndHandler =
                target => target.GetType().GetTypeInfo().DeclaredFields.ElementAt(1).GetValue(target);
            Func<object, object> getHandler =
                target => target.GetType().GetTypeInfo().DeclaredFields.ElementAt(0).GetValue(target);

            var chainedDelegateCloneAndHandler = getCapturedChainedDelegateClonedAndHandler(next.Target);
            var handler = getHandler(chainedDelegateCloneAndHandler);

            return handler.GetType().Name;
        }
    }

    public class NoUnitDuplicationRule : IHandler<string, int>
    {
        public int Handle(string input, Func<string, int> next)
        {
            var hasDuplication = input.Split(' ').Select(x => x.Last()).GroupBy(x => x).Any(g => g.Count() > 1);

            if (hasDuplication)
            {
                throw new Exception("What the heck");
            }

            return next(input);
        }
    }

    public class SAParser : Handler<string, int>, IWorkLogParser
    {
        public SAParser(Action<string> collector)
        {
            AddHandler(new IAmSpy(collector));
            AddHandler(new HourParser());
            AddHandler(new IAmSpy(collector));
            AddHandler(new MinuteParser());
        }

        public override int Handle(string input, Func<string, int> next) =>
            input.Split(' ').Select(piece => base.Handle(piece, next)).Sum();
    }

    public class MinuteParser : IWorkLogParser
    {
        private readonly string pattern = "^(\\d+)m$";

        public int Handle(string input, Func<string, int> next) =>
            Regex.IsMatch(input, pattern)
                ? int.Parse(Regex.Match(input, pattern).Groups[1].Value)
                : next(input);
    }

    public class HourParser : IWorkLogParser
    {
        private readonly string pattern = "^(\\d+)h$";

        public int Handle(string input, Func<string, int> next) =>
            Regex.IsMatch(input, pattern)
                ? int.Parse(Regex.Match(input, pattern).Groups[1].Value) * 60
                : next(input);
    }

    /// <summary>
    /// Service
    /// </summary>
    public interface IWorkLogParser : IHandler<string, int>
    {
    }
}
using System.Text;
using System.Text.RegularExpressions;

namespace GarageGenius.Common.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// LIKE-ähnliche Suche:
    /// - Unterstützt %, _ und * (wird wie % interpretiert).
    /// - Wenn keine Wildcards enthalten sind, wird case-insensitive Contains verwendet.
    /// </summary>

    public static bool Like(this string toSearch, string toFind)
    {
        if (toFind.IndexOfAny(['%', '*', '_']) < 0)
            return toSearch.Contains(toFind, StringComparison.OrdinalIgnoreCase);

        var sb = new StringBuilder();
        sb.Append(@"\A");

        foreach (var ch in toFind)
        {
            switch (ch)
            {
                case '%':
                case '*':
                    sb.Append(".*");
                    break;
                case '_':
                    sb.Append('.');
                    break;
                default:
                    sb.Append(Regex.Escape(ch.ToString()));
                    break;
            }
        }

        sb.Append(@"\z");
        return Regex.IsMatch(toSearch, sb.ToString(), RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }
}
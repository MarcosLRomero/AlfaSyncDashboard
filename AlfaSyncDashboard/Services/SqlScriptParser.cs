using System.Text.RegularExpressions;

namespace AlfaSyncDashboard.Services;

public static class SqlScriptParser
{
    private static readonly Regex GoRegex = new(@"^\s*GO\s*(?:--.*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public static IReadOnlyList<string> SplitBatches(string sql)
    {
        return GoRegex
            .Split(sql)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
}

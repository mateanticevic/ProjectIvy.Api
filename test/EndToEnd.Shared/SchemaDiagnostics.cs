using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ProjectIvy.EndToEnd;

public static class SchemaDiagnostics
{
    public static string Redact(string diagnostic, string sourceConnectionString)
    {
        var source = new SqlConnectionStringBuilder(sourceConnectionString);
        diagnostic = diagnostic.Replace(sourceConnectionString, "[redacted connection]", StringComparison.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(source.Password))
        {
            foreach (var password in new[] { source.Password.Replace("'", "''"), source.Password.Replace("\"", "\"\""), source.Password }
                .Distinct().OrderByDescending(x => x.Length))
                diagnostic = diagnostic.Replace(password, "[redacted]", StringComparison.Ordinal);
        }

        var host = Regex.Replace(source.DataSource, "^(tcp:|np:|lpc:)", "", RegexOptions.IgnoreCase).Split(',')[0];
        foreach (var value in new[] { source.DataSource, host, source.InitialCatalog, source.UserID, source.AttachDBFilename }
            .Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderByDescending(x => x.Length))
        {
            diagnostic = Regex.Replace(diagnostic,
                $"(?<![\\p{{L}}\\p{{N}}_]){Regex.Escape(value)}(?![\\p{{L}}\\p{{N}}_])",
                "[redacted]", RegexOptions.IgnoreCase);
        }
        return diagnostic;
    }
}

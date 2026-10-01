using System.Text.Json;
using System.Text.RegularExpressions;

namespace iw_foodhouse_md_web_scraper.DataCollectors;

public static class JsonScrapingHelper
{
    public static JsonDocument? ExtractJson(this string html, string regexPattern)
    {
        var match = Regex.Match(html, regexPattern, RegexOptions.Singleline);
        return match.Success ? JsonDocument.Parse(match.Groups[1].Value) : null;
    }

    public static string ExtractProperty(this JsonElement element, string propName)
    {
        return element.TryGetProperty(propName, out var property) ? property.ToString() : string.Empty;
    }
}

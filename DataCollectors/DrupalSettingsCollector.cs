using System.Text.Json;
using System.Text.RegularExpressions;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.Models;
namespace iw_foodhouse_md_web_scraper.DataCollectors;

public class DrupalSettingsCollector : IDrupalSettingsCollector
{
    public const string DrupalRestaurantsRegex = @"""restaurants_array""\s*:\s*(\{(?:[^{}]|(?<o>\{)|(?<-o>\}))*(?(o)(?!))\})";

    public IEnumerable<DrupalRestaurantData> ExtractRestaurants(string html)
    {
        var results = new List<DrupalRestaurantData>();

        var match = Regex.Match(html, DrupalRestaurantsRegex, RegexOptions.Singleline);
        if (match.Success)
        {
            using var json = JsonDocument.Parse(match.Groups[1].Value);

            if (json.RootElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in json.RootElement.EnumerateObject())
                {
                    results.Add(new DrupalRestaurantData(
                        ExtractProperty(prop.Value, "tid"),
                        ExtractProperty(prop.Value, "title")));
                }
            }
        }
        return results;
    }

    public IEnumerable<MenuItemDTO> ExtractMenuItems(string html)
    {
        throw new NotImplementedException();
    }

    private string ExtractProperty(JsonElement value, string propName)
    {
        return value.TryGetProperty(propName, out var property)
        ? property.GetString() ?? ""
        : "";
    }
}

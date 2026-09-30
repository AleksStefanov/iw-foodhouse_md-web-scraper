using System.Text.Json;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.Models;
namespace iw_foodhouse_md_web_scraper.DataCollectors;

public class DrupalSettingsCollector : IDrupalSettingsCollector
{
    public const string DrupalRestaurantsRegex = @"""restaurants_array""\s*:\s*(\{(?:[^{}]|(?<o>\{)|(?<-o>\}))*(?(o)(?!))\})";
    public const string DrupalMenuItemsRegex = @"jQuery\.extend\(Drupal\.settings,\s*(\{.*?\})\s*\);\s*</script>";

    public IEnumerable<RestaurantDto> ExtractRestaurants(string html)
    {
        var results = new List<RestaurantDto>();

        var json = html.ExtractJson(DrupalRestaurantsRegex);

        if (json != null && json.RootElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in json.RootElement.EnumerateObject())
            {
                results.Add(new RestaurantDto(
                    prop.Value.ExtractProperty("tid"),
                    prop.Value.ExtractProperty("title")));
            }
        }
        return results;
    }

    public IEnumerable<MenuItemDto> ExtractMenuItems(string html)
    {
        var results = new List<MenuItemDto>();
        
        var json = html.ExtractJson(DrupalMenuItemsRegex);
            
        if (json != null &&
            json.RootElement.TryGetProperty("delivery", out var deliveryNode) &&
            deliveryNode.TryGetProperty("nodes", out var itemsArray) &&
            itemsArray.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in itemsArray.EnumerateObject())
            {
                results.Add(new MenuItemDto(
                    prop.Value.ExtractProperty("tnid"),
                    prop.Value.ExtractProperty("restaurant_tid"),
                    prop.Value.ExtractProperty("title"),
                    prop.Value.ExtractProperty("total_price")));
            }
        }
        return results;
    }
}

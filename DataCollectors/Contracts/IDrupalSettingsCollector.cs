using iw_foodhouse_md_web_scraper.Models;

namespace iw_foodhouse_md_web_scraper.DataCollectors.Contracts;

public interface IDrupalSettingsCollector
{
    IEnumerable<RestaurantDto> ExtractRestaurants(string html);
    IEnumerable<MenuItemDto> ExtractMenuItems(string html);
}

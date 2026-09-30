using iw_foodhouse_md_web_scraper.Models;

namespace iw_foodhouse_md_web_scraper.DataCollectors.Contracts;

public interface IDrupalSettingsCollector
{
    IEnumerable<DrupalRestaurantData> ExtractRestaurants(string html);
    IEnumerable<MenuItemDTO> ExtractMenuItems(string html);
}

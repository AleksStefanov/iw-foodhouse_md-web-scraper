namespace iw_foodhouse_md_web_scraper.Models;

public record class MenuItemDTO(
    string Id,
    string RestaurantId,
    string Name,
    string Price);

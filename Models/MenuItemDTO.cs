using iw_foodhouse_md_web_scraper.DataPersisters.Contracts;

namespace iw_foodhouse_md_web_scraper.Models;

public record class MenuItemDto(
    string Id,
    string RestaurantId,
    string Name,
    string Price) : IPersistable;

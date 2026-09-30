namespace iw_foodhouse_md_web_scraper.Models;

public record class RestaurantDto(
    string Id,
    string Name,
    List<string>? Adresses = null);

namespace iw_foodhouse_md_web_scraper.Models;

public record class RestaurantDTO(
    string Id,
    string Name,
    List<string> Adresses);

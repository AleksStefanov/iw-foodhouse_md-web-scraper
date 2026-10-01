using iw_foodhouse_md_web_scraper.DataPersisters.Contracts;

namespace iw_foodhouse_md_web_scraper.Models;

public record class RestaurantDto(
    string Id,
    string Name,
    string Adresses = "") : IPersistable;

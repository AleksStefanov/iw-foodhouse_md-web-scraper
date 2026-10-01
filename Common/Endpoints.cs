namespace iw_foodhouse_md_web_scraper.Common;

public static class Endpoints
{
    private const string BaseUrl = "https://foodhouse.md/en";
    public const string RestaurantsUrl = $"{BaseUrl}/restaurants";
    public const string RestaurantDetailUrl = $"{BaseUrl}/restaurant/{{0}}";
}

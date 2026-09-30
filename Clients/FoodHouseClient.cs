using iw_foodhouse_md_web_scraper.Common;

namespace iw_foodhouse_md_web_scraper.Clients;

public class FoodHouseClient : IWebClient
{
    private readonly HttpClient client;

    public FoodHouseClient(HttpClient client)
    {
        this.client = client;
    }
    public Task<string> GetAllRestourantsHtmlPageAsync(CancellationToken cancellationToken = default)
    {
        return client.GetStringAsync(Constants.Http.RestaurantUri, cancellationToken);
    }
}

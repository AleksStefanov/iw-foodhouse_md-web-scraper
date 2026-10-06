namespace iw_foodhouse_md_web_scraper.Clients;

public interface IWebClient
{
    Task<string> GetAllRestaurantsHtmlPageAsync(CancellationToken cancellationToken = default);
    Task<string> GetRestourantDetailHtmlPageAsync(string id, CancellationToken cancellationToken = default);
}

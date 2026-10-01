namespace iw_foodhouse_md_web_scraper.Services.Contracts;

public interface IScrapingService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

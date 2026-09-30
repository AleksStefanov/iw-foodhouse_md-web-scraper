using iw_foodhouse_md_web_scraper.Clients;
using iw_foodhouse_md_web_scraper.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace iw_foodhouse_md_web_scraper.Services;

public class ScrapingService : IScrapingService
{
    private readonly IWebClient client;
    private readonly ILogger<IScrapingService> logger;
    
    public ScrapingService(
        IWebClient client,
        ILogger<IScrapingService> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching restaurants page..");
        var html = await client.GetAllRestourantsHtmlPageAsync(cancellationToken);

    }
}

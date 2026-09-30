using iw_foodhouse_md_web_scraper.Clients;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace iw_foodhouse_md_web_scraper.Services;

public class ScrapingService : IScrapingService
{
    private readonly IWebClient client;
    private readonly IDrupalSettingsCollector drupalCollector;
    private readonly ILogger<IScrapingService> logger;
    
    public ScrapingService(
        IWebClient client,
        IDrupalSettingsCollector drupalCollector,
        ILogger<IScrapingService> logger)
    {
        this.client = client;
        this.drupalCollector = drupalCollector;
        this.logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching restaurants page..");
        var html = await client.GetAllRestourantsHtmlPageAsync(cancellationToken);

        logger.LogInformation("Fetching restaurant ids & names..");
        var drupalRestaurantData = drupalCollector.ExtractRestaurants(html);
    }
}

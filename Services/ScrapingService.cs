using System.Collections.Concurrent;
using iw_foodhouse_md_web_scraper.Clients;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.Models;
using iw_foodhouse_md_web_scraper.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace iw_foodhouse_md_web_scraper.Services;

public class ScrapingService : IScrapingService
{
    private readonly IWebClient client;
    private readonly IDrupalSettingsCollector drupalCollector;
    private readonly IDomParser parser;
    private readonly ILogger<IScrapingService> logger;
    private readonly int parallelMax = 6;
    
    public ScrapingService(
        IWebClient client,
        IDrupalSettingsCollector drupalCollector,
        IDomParser parser,
        ILogger<IScrapingService> logger)
    {
        this.client = client;
        this.drupalCollector = drupalCollector;
        this.parser = parser;
        this.logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching restaurants page..");
        var html = await client.GetAllRestourantsHtmlPageAsync(cancellationToken);

        logger.LogInformation("Fetching restaurant ids & names..");
        var restaurants = drupalCollector.ExtractRestaurants(html);
        
        logger.LogInformation($"Fetching addresses and menu items..");

        var fullRestaurants = new ConcurrentBag<RestaurantDto>();
        var allMenuItems = new ConcurrentBag<MenuItemDto>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = parallelMax,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(restaurants, parallelOptions, async (restaurant, ct) =>
        {
            try
            {
                SimulateDelay(ct);

                logger.LogInformation($"Fetching '{restaurant.Name}' details page..");
                var detailHtml = await client.GetRestourantDetailHtmlPageAsync(restaurant.Id, ct);
                
                logger.LogInformation($"Collecting '{restaurant.Name}' addresses..");
                var addresses = parser.CollectAddresses(detailHtml);

                logger.LogInformation($"Collecting '{restaurant.Name}' menu items..");
                var items = drupalCollector.ExtractMenuItems(detailHtml);

                fullRestaurants.Add(new RestaurantDto(restaurant.Id, restaurant.Name, addresses));
                Parallel.ForEach(items, item => allMenuItems.Add(item));

                logger.LogInformation($"'{restaurant.Name}' data collected!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"'{restaurant.Name}' detailed data collection failed ..");
            }
        });

        logger.LogInformation($"Exporting restaurants to csv..");
        //await _exporter.ExportAsync(fullRestaurants, Constants.Output.RestaurantsCsv);

        logger.LogInformation($"SExporting menu items to csv..");
        //await _exporter.ExportAsync(allMenuItems, Constants.Output.MenuItemsCsv);

        logger.LogInformation("Data collection Successful");
    }

    private async void SimulateDelay(CancellationToken ct)
    {
        var delayMs = Random.Shared.Next(300, 1200);
        await Task.Delay(delayMs, ct);
    }
}

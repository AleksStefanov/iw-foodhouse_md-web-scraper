using System.Threading.Channels;
using iw_foodhouse_md_web_scraper.Clients;
using iw_foodhouse_md_web_scraper.Common;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.DataPersisters.Contracts;
using iw_foodhouse_md_web_scraper.Models;
using iw_foodhouse_md_web_scraper.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace iw_foodhouse_md_web_scraper.Services;

public class ScrapingService : IScrapingService
{
    private readonly IWebClient client;
    private readonly IDrupalSettingsCollector drupalCollector;
    private readonly IDomParser parser;
    private readonly IDataPersister persister;
    private readonly ILogger<IScrapingService> logger;
    private readonly int parallelMax = 6;
    
    public ScrapingService(
        IWebClient client,
        IDrupalSettingsCollector drupalCollector,
        IDomParser parser,
        IDataPersister persister,
        ILogger<IScrapingService> logger)
    {
        this.client = client;
        this.drupalCollector = drupalCollector;
        this.parser = parser;
        this.persister = persister;
        this.logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Fetching restaurants page..");
            var html = await client.GetAllRestourantsHtmlPageAsync(cancellationToken);

            logger.LogInformation("Fetching restaurant ids & names..");
            var restaurants = drupalCollector.ExtractRestaurants(html);
            
            var restaurantChannel = Channel.CreateUnbounded<RestaurantDto>(new UnboundedChannelOptions { SingleWriter = false, SingleReader = true });
            var menuChannel = Channel.CreateUnbounded<MenuItemDto>(new UnboundedChannelOptions { SingleWriter = false, SingleReader = true });

            var persistenceTasks = new[]
            {
                persister.PersistAsync(Output.RestaurantsFile, restaurantChannel.Reader, cancellationToken),
                persister.PersistAsync(Output.MenuItemsFile, menuChannel.Reader, cancellationToken)
            };

            await ProcessRestaurantsParallelAsync(restaurants, restaurantChannel.Writer, menuChannel.Writer, cancellationToken);
            
            restaurantChannel.Writer.Complete();
            menuChannel.Writer.Complete();

            await Task.WhenAll(persistenceTasks);
            logger.LogInformation("**** Successful Data collection ****");
        }
        catch(Exception ex)
        {
            logger.LogError(ex, $"'Restourants collection failed ..");
        }
    }

    private async Task ProcessRestaurantsParallelAsync(
        IEnumerable<RestaurantDto> restaurants,
        ChannelWriter<RestaurantDto> rChannelWriter,
        ChannelWriter<MenuItemDto> miChannelWriter,
        CancellationToken cancellationToken)
    {
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

                var restaurantDto = new RestaurantDto(restaurant.Id, restaurant.Name, string.Join("; ", addresses));

                logger.LogInformation($"Exporting restaurants to csv..");
                await rChannelWriter.WriteAsync(restaurantDto, ct);
                
                logger.LogInformation($"Exporting menu items to csv..");
                foreach (var item in items)
                {
                    await miChannelWriter.WriteAsync(item, ct);
                }

                logger.LogInformation($"'{restaurant.Name}' data collected!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"'{restaurant.Name}' detailed data collection failed ..");
            }
        });
    }

    private async void SimulateDelay(CancellationToken ct)
    {
        var delayMs = Random.Shared.Next(300, 1200);
        await Task.Delay(delayMs, ct);
    }
}
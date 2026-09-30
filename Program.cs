using AngleSharp.Html.Parser;
using iw_foodhouse_md_web_scraper.Clients;
using iw_foodhouse_md_web_scraper.DataCollectors;
using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using iw_foodhouse_md_web_scraper.Services;
using iw_foodhouse_md_web_scraper.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
.ConfigureServices((context, services) =>
{
    services.AddHttpClient<IWebClient,FoodHouseClient>();

    services.AddTransient<IDrupalSettingsCollector,DrupalSettingsCollector>();
    services.AddTransient<IDomParser, DomParser>();
    services.AddTransient<IScrapingService,ScrapingService>();
    services.AddSingleton<IHtmlParser, HtmlParser>();

}).Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var scraper = host.Services.GetRequiredService<IScrapingService>();

try
{
    logger.LogInformation("Commencing scraping..");
    await scraper.RunAsync();
}
catch(Exception ex)
{
    logger.LogError(ex, "Critical error occured");
    Environment.ExitCode = 1;
}
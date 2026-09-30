namespace iw_foodhouse_md_web_scraper.DataCollectors.Contracts;

public interface IDomParser
{
    List<string> CollectAddresses(string html);
}

using iw_foodhouse_md_web_scraper.DataCollectors.Contracts;
using AngleSharp.Html.Parser;

namespace iw_foodhouse_md_web_scraper.DataCollectors;

public class DomParser : IDomParser
{
    private readonly IHtmlParser parser;
    private const string addressSelector = ".restaurant-addresses-block .restaurant-address span";

    public DomParser(IHtmlParser parser)
    {
        this.parser = parser;
    }
    
    public List<string> CollectAddresses(string html)
    {
            return parser.ParseDocument(html)
            .QuerySelectorAll(addressSelector)
            .Select(node => node.TextContent)
            .ToList();       
    }
}
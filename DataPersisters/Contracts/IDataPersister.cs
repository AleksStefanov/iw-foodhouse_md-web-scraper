using System.Threading.Channels;

namespace iw_foodhouse_md_web_scraper.DataPersisters.Contracts;

public interface IDataPersister
{
    Task PersistAsync<T>(string destination, ChannelReader<T> reader, CancellationToken cancellationToken)
    where T : IPersistable;
}

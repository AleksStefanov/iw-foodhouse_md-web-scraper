using System.Reflection;
using System.Threading.Channels;
using iw_foodhouse_md_web_scraper.DataPersisters.Contracts;

namespace iw_foodhouse_md_web_scraper.DataPersisters;

public class CsvPersister : IDataPersister
{
    public async Task PersistAsync<T>(string fileName, ChannelReader<T> reader, CancellationToken cancellationToken)
    where T : IPersistable
    {
       using var fileStream = new FileStream(
            fileName,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);
        
        using var writer = new StreamWriter(fileStream, leaveOpen: false);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties.Length == 0) return;

        var headers = properties.Select(p => p.Name);

        await writer.WriteLineAsync(string.Join(",", headers));

        await foreach (var item in reader.ReadAllAsync(cancellationToken))
        {
            var values = properties.Select(p => 
            {
                var rawValue = p.GetValue(item, null);
                return EscapeCsvField(rawValue?.ToString() ?? string.Empty);
            });

            await writer.WriteLineAsync(string.Join(",", values));
        }
    }

    private string EscapeCsvField(string field)
    {
            return $"\"{field.Replace("\"", "\"\"")}\"";
    }
}

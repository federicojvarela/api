using FJVApp.Fetchers;
using FJVApp.Processors;
using FJVApp.Reporters;


class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();

        // Instantiate the necessary components
        var matcher = new RecordMatcher();
        var reporter = new RecordReporter(httpClient, "http://localhost:7299/sink/a");
        var xmlFetcher = new XmlRecordFetcher(httpClient);
        var jsonFetcher = new JsonRecordFetcher(httpClient);

        var processor = new RecordProcessor(jsonFetcher, xmlFetcher, matcher, reporter);
        await processor.ProcessRecordsAsync();

        Console.WriteLine("Processing completed.");
    }
}

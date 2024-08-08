using FJVApp.Fetchers;
using FJVApp.Processors;
using FJVApp.Reporters;


class Program
{
    static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        const string SinkEndpoint = "http://localhost:7299/sink/a";
        // Instantiate the necessary components
        var matcher = new RecordMatcher();
        var reporter = new RecordReporter(httpClient, SinkEndpoint);
        var xmlFetcher = new XmlRecordFetcher(httpClient);
        var jsonFetcher = new JsonRecordFetcher(httpClient);

        var processor = new RecordProcessor(jsonFetcher, xmlFetcher, matcher, reporter);
        await processor.ProcessRecordsAsync();

        Console.WriteLine("Processing completed.");
    }
}

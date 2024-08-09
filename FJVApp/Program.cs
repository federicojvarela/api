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
        var matcher = new RecordMatcher(); // Creating a matcher for records
        var reporter = new RecordReporter(httpClient, SinkEndpoint); // Creating a reporter with the HttpClient and endpoint
        var xmlFetcher = new XmlRecordFetcher(httpClient); // Creating a fetcher for XML records
        var jsonFetcher = new JsonRecordFetcher(httpClient); // Creating a fetcher for JSON records

        var processor = new RecordProcessor(jsonFetcher, xmlFetcher, matcher, reporter); // Creating a processor with the fetchers and matcher
        await processor.ProcessRecordsAsync(); // Processing records asynchronously

        Console.WriteLine("Processing completed.");
    }
}
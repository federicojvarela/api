using FJVApp.Fetchers;
using FJVApp.Interfaces;
using FJVApp.Models;
using FJVApp.Processors;
using FJVApp.Reporters;
using System.Net.Http;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var jsonFetcher = new JsonRecordFetcher(httpClient);
        var xmlFetcher = new XmlRecordFetcher(httpClient);
        var matcher = new RecordMatcher();
        var reporter = new RecordReporter(httpClient, "http://localhost:7299/sink/a");

        var processor = new RecordProcessor(jsonFetcher, xmlFetcher, matcher, reporter);
        await processor.ProcessRecordsAsync();
        Console.WriteLine("Processing completed.");
    }
}

public class RecordProcessor
{
    private readonly IRecordFetcher jsonFetcher;
    private readonly IRecordFetcher xmlFetcher;
    private readonly IRecordMatcher matcher;
    private readonly IRecordReporter reporter;

    public RecordProcessor(IRecordFetcher jsonFetcher, IRecordFetcher xmlFetcher, RecordMatcher matcher, RecordReporter reporter)
    {
        this.jsonFetcher = jsonFetcher;
        this.xmlFetcher = xmlFetcher;
        this.matcher = matcher;
        this.reporter = reporter;
    }

    public async Task ProcessRecordsAsync()
    {
        List<Record> recordsA = await jsonFetcher.FetchRecordsAsync("http://localhost:7299/source/a");
        List<Record> recordsB = await xmlFetcher.FetchRecordsAsync("http://localhost:7299/source/b");

        matcher.MatchRecords(recordsA, recordsB, out var joinedRecords, out var orphanedRecords);
        await reporter.ReportRecordsAsync(joinedRecords);
        await reporter.ReportRecordsAsync(orphanedRecords);
    }
}

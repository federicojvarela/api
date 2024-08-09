using FJVApp.Interfaces;
using FJVApp.Models;

namespace FJVApp.Processors
{
    public class RecordProcessor
    {
        private const string JsonEndpoint = "http://localhost:7299/source/a"; // Constant for JSON endpoint
        private const string XmlEndpoint = "http://localhost:7299/source/b"; // Constant for XML endpoint

        private readonly IRecordFetcher jsonFetcher;
        private readonly IRecordFetcher xmlFetcher;
        private readonly IRecordMatcher matcher;
        private readonly IRecordReporter reporter;

        public RecordProcessor(IRecordFetcher jsonFetcher, IRecordFetcher xmlFetcher, IRecordMatcher matcher, IRecordReporter reporter)
        {
            this.jsonFetcher = jsonFetcher;
            this.xmlFetcher = xmlFetcher;
            this.matcher = matcher;
            this.reporter = reporter;
        }

        public async Task ProcessRecordsAsync()
        {
            var recordsA = new List<Record>();
            var recordsB = new List<Record>();
            bool doneA = false;
            bool doneB = false;

            while (!doneA || !doneB)
            {
                // Fetch one record from JSON endpoint
                if (!doneA)
                {
                    var newRecordA = await FetchSingleRecord(jsonFetcher, JsonEndpoint); // Use constant

                    if (newRecordA.Status.Contains("done")) // Validate for "done"
                    {
                        doneA = true;
                    }
                    else
                    {
                        if (newRecordA != null)
                            recordsA.Add(newRecordA);
                    }
                }

                // Fetch one record from XML endpoint
                if (!doneB)
                {
                    var newRecordB = await FetchSingleRecord(xmlFetcher, XmlEndpoint); // Use constant

                    if (newRecordB.Status.Contains("done"))
                    {
                        doneB = true;
                    }
                    else
                    {
                        if (newRecordB != null)
                            recordsB.Add(newRecordB);
                    }
                }

                // Immediately process joined records
                var (joinedRecords, orphaned) = matcher.CategorizeRecords(recordsA, recordsB);
                if (joinedRecords.Count > 0)
                {
                    await reporter.ReportRecordsAsync(joinedRecords);
                    joinedRecords.ForEach(r =>
                    {
                        recordsA.RemoveAll(rec => rec.Id == r.id);
                        recordsB.RemoveAll(rec => rec.Id == r.id);
                    });
                }
            }

            // Once done, process remaining records as orphaned
            Console.WriteLine("....................Processing remaining orphaned records...");
            var (_, orphanedRecords) = matcher.CategorizeRecords(recordsA, recordsB);
            if (orphanedRecords.Count > 0)
            {
                await reporter.ReportRecordsAsync(orphanedRecords);
            }
        }

        private async Task<Record> FetchSingleRecord(IRecordFetcher fetcher, string url)
        {
            int retryCount = 3;
            while (retryCount > 0)
            {
                try
                {
                    var records = await fetcher.FetchRecordsAsync(url);
                    return records != null && records.Count > 0 ? records[0] : new Record(); // Return the first record or a new Record if none
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error fetching records");
                    retryCount--;
                    if (retryCount == 0)
                    {
                        throw; // Rethrow the exception if all retries fail
                    }
                    await Task.Delay(1000); // Wait before retrying
                }
            }
            return new Record();
        }
    }
}
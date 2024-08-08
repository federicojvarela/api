using FJVApp.Interfaces;
using FJVApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FJVApp.Processors
{
    public class RecordProcessor
    {
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
                    var newRecordA = await FetchSingleRecord(jsonFetcher, "http://localhost:7299/source/a");
                    Console.WriteLine("-------------------------DATA-A" + newRecordA.Data);
                    if (newRecordA.Data.Contains("done")) // Validate for "done"
                    {
                        doneA = true;
                        // throw new InvalidOperationException("Processing JSON completed."); // Stop execution
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
                    var newRecordB = await FetchSingleRecord(xmlFetcher, "http://localhost:7299/source/b");
                    Console.WriteLine("-------------------------DATA-B" + newRecordB.Data);
                    if (newRecordB.Data.Contains("done"))
                    {
                        doneB = true;
                        // throw new InvalidOperationException("Processing XML completed."); // Stop execution
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching records: {ex.Message}");
                    retryCount--;
                    if (retryCount == 0)
                    {
                        throw; // Rethrow the exception if all retries fail
                    }
                    await Task.Delay(1000); // Wait before retrying
                }
            }
            return null; // Fallback return
        }
    }
}
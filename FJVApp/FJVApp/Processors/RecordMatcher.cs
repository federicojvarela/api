using FJVApp.Interfaces;
using FJVApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FJVApp.Processors
{
    public class RecordMatcher : IRecordMatcher
    {
        public void MatchRecords(List<Record> recordsA, List<Record> recordsB, out List<SinkRecord> joinedRecords, out List<SinkRecord> orphanedRecords)
        {
            joinedRecords = new List<SinkRecord>();
            orphanedRecords = new List<SinkRecord>();

            var recordBLookup = new ConcurrentDictionary<string, Record>();

            // Add all B records to a concurrent dictionary for fast lookups
            foreach (var recordB in recordsB)
            {
                if (IsValid(recordB))
                {
                    // Normalize ID to ensure consistency
                    var normalizedId = NormalizeId(recordB.Id);
                    recordBLookup[normalizedId] = recordB;
                    Console.WriteLine($"Added to recordBLookup: {normalizedId}");
                }
            }

            // Process A records
            foreach (var recordA in recordsA)
            {
                if (!IsValid(recordA))
                {
                    continue; // Skip defective records
                }

                // Normalize ID for consistency
                var normalizedId = NormalizeId(recordA.Id);

                if (recordBLookup.TryRemove(normalizedId, out _))
                {
                    Console.WriteLine($"Match found and removed for ID: {recordA.Id}");
                    joinedRecords.Add(new SinkRecord { id = recordA.Id, kind = "joined" });
                }
                else
                {
                    Console.WriteLine($"No match found for ID: {recordA.Id}");
                    orphanedRecords.Add(new SinkRecord { id = recordA.Id, kind = "orphaned" });
                }
            }

            // Remaining B records are orphaned
            foreach (var orphanedBRecord in recordBLookup.Values)
            {
                Console.WriteLine($"Orphaned Record B ID: {orphanedBRecord.Id}");
                orphanedRecords.Add(new SinkRecord { id = orphanedBRecord.Id, kind = "orphaned" });
            }
        }

        private bool IsValid(Record record)
        {
            return record != null && !string.IsNullOrEmpty(record.Id);
        }

        private string NormalizeId(string id)
        {
            return id.Trim();
        }
    }
}

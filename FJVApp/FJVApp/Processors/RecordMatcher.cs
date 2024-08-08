using FJVApp.Interfaces;
using FJVApp.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FJVApp.Processors
{
    public class RecordMatcher : IRecordMatcher
    {
        private List<SinkRecord> orphanedRecords = new List<SinkRecord>(); // Moved to class-level

        public (List<SinkRecord> Joined, List<SinkRecord> Orphaned) CategorizeRecords(List<Record> recordsA, List<Record> recordsB)
        {
            var joinedRecords = new List<SinkRecord>();
            var lookupA = recordsA
                .Where(IsValid)
                .ToDictionary(r => NormalizeId(r.Id), r => r);

            var lookupB = recordsB
                .Where(IsValid)
                .ToDictionary(r => NormalizeId(r.Id), r => r);

            Console.WriteLine("Starting categorization of records...");

            // Find joined records
            foreach (var record in lookupA)
            {
                if (lookupB.Remove(record.Key, out var _))
                {
                    joinedRecords.Add(new SinkRecord { id = record.Key, kind = "joined" });
                    Console.WriteLine($"Record {record.Key} is joined.");
                }
                else
                {
                    // Check if the record is in orphanedRecords
                    var orphanedRecord = orphanedRecords.FirstOrDefault(o => o.id == record.Key);
                    if (orphanedRecord != null)
                    {
                        joinedRecords.Add(new SinkRecord { id = record.Key, kind = "joined" });
                        orphanedRecords.Remove(orphanedRecord); // Remove from orphanedRecords
                        Console.WriteLine($"Record {record.Key} is joined (was orphaned).");
                    }
                    else
                    {
                        orphanedRecords.Add(new SinkRecord { id = record.Key, kind = "orphaned" });
                        Console.WriteLine($"Record {record.Key} is orphaned (in A only).");
                    }
                }
            }

            // Remaining records in lookupB are orphaned
            foreach (var record in lookupB.Values)
            {
                // Check if the record is in orphanedRecords
                var orphanedRecord = orphanedRecords.FirstOrDefault(o => o.id == record.Id);
                if (orphanedRecord != null)
                {
                    joinedRecords.Add(new SinkRecord { id = record.Id, kind = "joined" });
                    orphanedRecords.Remove(orphanedRecord); // Remove from orphanedRecords
                    Console.WriteLine($"Record {record.Id} is joined (was orphaned).");
                }
                else
                {
                    orphanedRecords.Add(new SinkRecord { id = record.Id, kind = "orphaned" });
                    Console.WriteLine($"Record {record.Id} is orphaned (in B only).");
                }
            }

            Console.WriteLine($"Categorization complete. Joined: {joinedRecords.Count}, Orphaned: {orphanedRecords.Count}");

            // Clear recordsA and recordsB after processing
            recordsA.Clear();
            recordsB.Clear();

            // Immediately return joined records
            return (joinedRecords, orphanedRecords);
        }

        // Method to send orphaned records at the end
        public List<SinkRecord> GetOrphanedRecords()
        {
            return orphanedRecords; // Return all orphaned records
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
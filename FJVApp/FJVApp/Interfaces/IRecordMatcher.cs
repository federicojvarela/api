using FJVApp.Models;
using System.Collections.Generic;

namespace FJVApp.Interfaces
{
    public interface IRecordMatcher
    {
        (List<SinkRecord> Joined, List<SinkRecord> Orphaned) CategorizeRecords(List<Record> recordsA, List<Record> recordsB);
    }
}

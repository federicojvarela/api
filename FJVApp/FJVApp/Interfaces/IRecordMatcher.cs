using FJVApp.Models;
using System.Collections.Generic;

namespace FJVApp.Interfaces
{
    public interface IRecordMatcher
    {
        void MatchRecords(List<Record> recordsA, List<Record> recordsB, out List<SinkRecord> joinedRecords, out List<SinkRecord> orphanedRecords);
    }
}

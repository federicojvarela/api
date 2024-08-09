using FJVApp.Models;

namespace FJVApp.Interfaces
{
    public interface IRecordReporter
    {
        Task ReportRecordsAsync(List<SinkRecord> records);
    }
}

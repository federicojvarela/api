using FJVApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FJVApp.Interfaces
{
    public interface IRecordReporter
    {
        Task ReportRecordsAsync(List<SinkRecord> records);
    }
}

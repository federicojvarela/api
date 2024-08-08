using FJVApp.Models;

namespace FJVApp.Interfaces
{
    public interface IRecordFetcher
    {
        Task<List<Record>> FetchRecordsAsync(string url);
    }
}

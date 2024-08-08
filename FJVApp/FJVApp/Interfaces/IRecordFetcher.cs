using FJVApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FJVApp.Interfaces
{
    public interface IRecordFetcher
    {
        Task<List<Record>> FetchRecordsAsync(string url);
    }
}

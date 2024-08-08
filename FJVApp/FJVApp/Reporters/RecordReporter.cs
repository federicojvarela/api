using FJVApp.Interfaces;
using FJVApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FJVApp.Reporters
{
    public class RecordReporter : IRecordReporter
    {
        private readonly HttpClient httpClient;
        private readonly string sinkUrl;

        public RecordReporter(HttpClient httpClient, string sinkUrl)
        {
            this.httpClient = httpClient;
            this.sinkUrl = sinkUrl;
        }

        public async Task ReportRecordsAsync(List<SinkRecord> records)
        {
            foreach (var record in records)
            {
                // Serialize each record to JSON
                var jsonContent = JsonSerializer.Serialize(record);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Send the POST request
                try
                {
                    var response = await httpClient.PostAsync(sinkUrl, content);

                    // Log if the response is not successful
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to send record {record.id}. Status Code: {response.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine("Record sent: " + record.id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while sending record {record.id}: {ex.Message}");
                }
            }
        }
    }
}

using FJVApp.Interfaces;
using FJVApp.Models;
using System.Net;
using System.Text.Json;

namespace FJVApp.Fetchers
{
    public class JsonRecordFetcher : IRecordFetcher
    {
        private readonly HttpClient httpClient;

        public JsonRecordFetcher(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Record>> FetchRecordsAsync(string url)
        {
            var records = new List<Record>();

            try
            {
                string response = await httpClient.GetStringAsync(url);

                // Deserialize the JSON response
                var jsonResponse = JsonSerializer.Deserialize<JsonResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (jsonResponse != null)
                {
                    // Check for "done" message
                    if (jsonResponse.Status.ToLower() == "done" || jsonResponse.Id.ToLower() == "done")
                    {
                        var doneRecord = new Record
                        {
                            Id = jsonResponse.Id,
                            Status = jsonResponse.Status
                        };
                        records.Add(doneRecord);
                        return records;
                    }

                    // Convert JsonResponse to Record and add to the list
                    var record = new Record
                    {
                        Id = jsonResponse.Id,
                        Status = jsonResponse.Status
                    };

                    if (IsValid(record))
                    {
                        records.Add(record);
                        Console.WriteLine("Record JSON added to list: " + record.Id);
                    }
                    else
                    {
                        Console.WriteLine("Skipping defective JSON record.");
                    }
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotAcceptable)
            {
                Console.WriteLine("Received 406 status code in JsonRecordFetcher.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching JSON records: {ex.Message}");
            }

            return records;
        }

        private bool IsValid(Record record)
        {
            return record != null && !string.IsNullOrEmpty(record.Id);
        }
    }
}

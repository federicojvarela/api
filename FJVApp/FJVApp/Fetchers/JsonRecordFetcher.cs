using FJVApp.Interfaces;
using FJVApp.Models;
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
            int count = 0;

            while (true)
            {
                count++;
                var response = await httpClient.GetStringAsync(url);

                // Check for "done" message in the response
                if (response.Contains("done"))
                {
                    break;
                }

                try
                {
                    // Deserialize the JSON response into a JsonResponse object
                    var jsonResponse = JsonSerializer.Deserialize<JsonResponse>(response, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (jsonResponse != null && jsonResponse.Status == "ok")
                    {
                        // Convert JsonResponse to Record and add to the list
                        var record = new Record
                        {
                            Id = jsonResponse.Id,
                            Data = jsonResponse.Status
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
                catch (JsonException)
                {
                    count--;
                    Console.WriteLine($"Error parsing JSON response");
                }
                if (count == 10)
                {
                    break;
                }
            }

            return records;
        }

        private bool IsValid(Record record)
        {
            return record != null && !string.IsNullOrEmpty(record.Id);
        }
    }
}

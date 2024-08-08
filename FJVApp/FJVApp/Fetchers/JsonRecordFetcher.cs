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
                var response = await httpClient.GetStringAsync(url);
                var jsonResponse = DeserializeJsonResponse(response);

                if (jsonResponse != null)
                {
                    if (IsDoneMessage(jsonResponse))
                    {
                        records.Add(CreateRecord(jsonResponse));
                        return records;
                    }

                    var record = CreateRecord(jsonResponse);
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
            catch (Exception)
            {
                Console.WriteLine("Error fetching JSON records");
            }

            return records;
        }

        private JsonResponse DeserializeJsonResponse(string response)
        {
            return JsonSerializer.Deserialize<JsonResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new JsonResponse(); // Ensure a non-null return
        }

        private bool IsDoneMessage(JsonResponse jsonResponse)
        {
            return string.Equals(jsonResponse.Status, "done", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(jsonResponse.Id, "done", StringComparison.OrdinalIgnoreCase);
        }

        private Record CreateRecord(JsonResponse jsonResponse)
        {
            return new Record
            {
                Id = jsonResponse.Id,
                Status = jsonResponse.Status
            };
        }

        private bool IsValid(Record record)
        {
            return record != null && !string.IsNullOrEmpty(record.Id);
        }
    }
}
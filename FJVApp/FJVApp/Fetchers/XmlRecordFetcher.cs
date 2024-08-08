using FJVApp.Interfaces;
using FJVApp.Models;
using System.Xml;
using System.Xml.Linq;

namespace FJVApp.Fetchers
{
    public class XmlRecordFetcher : IRecordFetcher
    {
        private readonly HttpClient httpClient;

        public XmlRecordFetcher(HttpClient httpClient)
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
                string? response = null;
                try
                {
                    response = await httpClient.GetStringAsync(url);

                    // Check for "done" message in the response
                    if (response.Contains("<done>"))
                    {
                        break;
                    }

                    var xDocument = XDocument.Parse(response);

                    // Extract records from XML
                    if (xDocument.Root != null)
                    {
                        foreach (var element in xDocument.Root.Elements("id"))
                        {
                            var record = new Record
                            {
                                Id = element.Attribute("value")?.Value ?? string.Empty,
                                Data = "ok"
                            };

                            if (IsValid(record))
                            {
                                Console.WriteLine("Record XML added to list: " + record.Id);
                                records.Add(record);
                            }
                            else
                            {
                                Console.WriteLine("Skipping defective XML record.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("XML document has no root element.");
                    }
                }
                catch (XmlException)
                {
                    count--;
                    Console.WriteLine($"Error parsing XML response");
                }
                catch (Exception)
                {
                    count--;
                    Console.WriteLine($"Unexpected error");
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

﻿using FJVApp.Interfaces;
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


            string? response = null; // Use nullable string

            try
            {
                // Fetch the XML string from the specified URL
                response = await httpClient.GetStringAsync(url);

                // Check for "done" message in the response
                if (response.Contains("done"))
                {
                    Console.WriteLine("Received 'done' message. Ending fetch loop.");
                    List<Record> doneRecords = new List<Record>();
                    doneRecords.Add(new Record { Id = "done", Status = "done" });
                    return doneRecords;
                }

                var xDocument = XDocument.Parse(response);

                // Extract records from XML
                if (xDocument.Root != null)
                {
                    foreach (var element in xDocument.Root.Elements("id"))
                    {
                        var id = element.Attribute("value")?.Value;

                        if (!string.IsNullOrEmpty(id))
                        {
                            var record = new Record
                            {
                                Id = id,
                                Status = "ok"
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
                        else
                        {
                            Console.WriteLine("No 'value' attribute found in 'id' element.");
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
                Console.WriteLine("Error parsing XML response.");
            }
            catch (Exception)
            {
                Console.WriteLine("Unexpected error");
            }

            return records;
        }

        private bool IsValid(Record record)
        {
            return record != null && !string.IsNullOrEmpty(record.Id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Wiki___Footballer_Statics.ExternalClasses;

namespace Wiki___Footballer_Statics.Services.Concrete
{
    public static class WikiDataService
    {
        public static async Task<List<WikiDataResponse>> GetPlayerDetails(params int[] mID)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10) // Set the timeout to 10 minutes
            };
            client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.34.0");

            var idValues = string.Join(" ", mID.Select(id => $"\"{id}\""));
            var responses = new List<WikiDataResponse>();

            // Define the batch size
            var batchSize = 100;

            for (int i = 0; i < mID.Length; i += batchSize)
            {
                var batch = mID.Skip(i).Take(batchSize).ToArray();
                var batchIdValues = string.Join(" ", batch.Select(id => $"\"{id}\""));

                var query = $@"
        SELECT ?nationLabel ?positionLabel ?birthDate ?id ?formattedName
        WHERE {{
            VALUES ?id {{ {batchIdValues} }}
            ?item wdt:P2458 ?id ;
                  wdt:P27 ?nation .
            OPTIONAL {{ ?item wdt:P413 ?position }}  # Pozisyon
            OPTIONAL {{ ?item wdt:P569 ?birthDate }} # Doğum tarihi
            OPTIONAL {{ ?item rdfs:label ?nameTr . FILTER(LANG(?nameTr) = 'tr') }}
            OPTIONAL {{ ?item rdfs:label ?nameEn . FILTER(LANG(?nameEn) = 'en') }}
OPTIONAL {{ 
    ?wikiLink schema:about ?item ;
              schema:isPartOf <https://tr.wikipedia.org/> ;
              schema:name ?wikiTitle .
  }}
            BIND(
                COALESCE(
                    IF(BOUND(?wikiTitle),
                    IF(CONTAINS(str(?wikiTitle), '('),
                        CONCAT(str(?wikiTitle), '|', REPLACE(str(?wikiTitle), '\\s*\\(.*\\)', '')),
                        CONCAT(str(?wikiTitle))
                    ),
                    IF(BOUND(?nameTr),
                        IF(CONTAINS(str(?nameTr), '('),
                            CONCAT(str(?nameTr), '|', REPLACE(str(?nameTr), '\\s*\\(.*\\)', '')),
                            CONCAT(str(?nameTr))
                        ),
                        IF(BOUND(?nameEn),
                            IF(CONTAINS(str(?nameEn), '('),
                                CONCAT(str(?nameEn), '|', REPLACE(str(?nameEn), '\\s*\\(.*\\)', '')),
                                CONCAT(str(?nameEn))
                            ),
                            ''  
                        )
                    )
                )) AS ?formattedName
            )
            SERVICE wikibase:label {{ bd:serviceParam wikibase:language 'tr,en' . }}
        }}
        ORDER BY ?formattedName
    ";

                try
                {
                    var response = await client.GetFromJsonAsync<WikiDataResponse>($"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(query)}&format=json");
                    if (response != null)
                    {
                        responses.Add(response);
                    }
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    // Handle timeout exception
                    Console.WriteLine("The request timed out.");
                }
            }

            return responses;
        }


    }
}

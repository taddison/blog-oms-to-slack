#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Collections;
using Newtonsoft.Json;
using System.Text;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    var slackUri = "https://requestb.in/1hoo74a1";
    var slackChannel = "#oms-alerts";
    var slackUsername = "OMS";

    var data = await req.Content.ReadAsAsync<OMSPayload>();

    var warningThreshold = data?.WarningThreshold ?? 75;
    var criticalThreshold = data?.CriticalThreshold ?? 90;

    var aggregatedResults = data.SearchResults.value.GroupBy(v => v.Computer)
                            .Select(g => new {  Computer = g.Key
                                                ,Average = g.Average(v => v.AggregatedValue)
                                                ,Warning = g.Count(v => v.AggregatedValue >= warningThreshold)
                                                ,Critical = g.Count(v => v.AggregatedValue >= criticalThreshold) });

    var message = string.Empty;
    var critical = false;
    
    foreach(var result in aggregatedResults)
    {
        message +=  $" - {result.Computer}:[{result.Warning} >{warningThreshold}% | {result.Critical} >{criticalThreshold}% | {(result.Average/100):P0} avg]";
        
        if(result.Critical > 0)
        {
            critical = true;
        }
    }

    message = $"Infra - CPU {(critical ? "Critical" : "Warning")}" + message;

    using(var client = new HttpClient())
    {
        var slackPayload = new SlackMessage() { text = message, channel = slackChannel, username = slackUsername, link_names = true };
        var hook = new StringContent(JsonConvert.SerializeObject(slackPayload), Encoding.UTF8, "application/json");
        var resp = await client.PostAsync(slackUri, hook);
    }

    return req.CreateResponse(HttpStatusCode.OK);
}

public class SlackMessage
{
    public string text { get; set; }
    public string channel { get; set; }
    public string username { get; set; }
    public bool link_names { get; set ; }
}

public class Metadata
{
    public long top { get; set; }
    public string RequestId { get; set; }
    public string Status { get; set; }
    public int NumberOfDocuments { get; set; }
    public string StartTime { get; set; }
    public string LastUpdated { get; set; }
    public string ETag { get; set; }
    public string aggregateIntervalField { get; set; }
    public double aggregateIntervalSeconds { get; set; }
    public string resultType { get; set; }
    public string aggregatedValueField { get; set; }
    public List<string> aggregatedValueFields { get; set; }
    public List<string> aggregateGroupingFields { get; set; }
    public double sum { get; set; }
    public double max { get; set; }
    public int total { get; set; }
    public int requestTime { get; set; }
}

public class Value
{
    public string TimeGenerated { get; set; }
    public string Computer { get; set; }
    public double AggregatedValue { get; set; }
}

public class SearchResults
{
    public string id { get; set; }
    public Metadata __metadata { get; set; }
    public List<Value> value { get; set; }
}

public class OMSPayload
{
    public bool IncludeSearchResults { get; set; }
    public SearchResults SearchResults { get; set; }
    public int? WarningThreshold { get; set; }
    public int? CriticalThreshold { get; set; }
}
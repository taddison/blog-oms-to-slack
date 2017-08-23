using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs;

public class DriveToSlack
{
    [FunctionName("DriveToSlack")]
    public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
    {
        var slackUri = "https://requestb.in/1hoo74a1";
        var slackChannel = "#oms-alerts";
        var slackUsername = "OMS";

        var data = await req.Content.ReadAsAsync<OMSPayload>();

        var warningThreshold = data?.WarningThreshold ?? 10;
        var criticalThreshold = data?.CriticalThreshold ?? 5;

        var aggregatedResults = data.SearchResults.Tables[0].Rows.GroupBy(r => r[1].ToString())
                                .Select(g => new
                                {
                                    Computer = g.Key
                                                    ,
                                    Min = g.Min(r => Double.Parse(r[2].ToString()))
                                                    ,
                                    Warning = g.Count(r => Double.Parse(r[2].ToString()) <= warningThreshold)
                                                    ,
                                    Critical = g.Count(r => Double.Parse(r[2].ToString()) <= criticalThreshold)
                                });

        var message = string.Empty;
        var critical = false;

        foreach (var result in aggregatedResults)
        {
            message += $" - {result.Computer} : [{(result.Min / 100):P1} free]";

            if (result.Critical > 0)
            {
                critical = true;
            }
        }

        message = $"Infra - Drive Space {(critical ? "Critical" : "Warning")}" + message;

        using (var client = new HttpClient())
        {
            var slackPayload = new SlackMessage() { Text = message, Channel = slackChannel, Username = slackUsername, LinkNames = true };
            var hook = new StringContent(JsonConvert.SerializeObject(slackPayload), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync(slackUri, hook);
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }
}
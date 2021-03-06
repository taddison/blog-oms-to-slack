using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using OMSToSlack;
using OMSToSlack.Models;
using OMSToSlack.Models.OMS;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class OMSMetricToSlack
{
    private static AlertProcessor __processor;

    [FunctionName("OMSMetricToSlack")]
    public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
    {
        if(__processor == null)
        {
            __processor = new AlertProcessor(context);
        }
        var data = await req.Content.ReadAsAsync<OMSPayload>();

        var metrics = data.SearchResults.Tables[0].Rows.Select(r =>
        {
            return new MetricValue(DateTime.Parse(r[0].ToString()), Double.Parse(r[2].ToString()));
        });

        // Server1|E:
        var machineName = data.SearchResults.Tables[0].Rows[0][1].ToString();
        string instanceName = null;

        if(machineName.Contains('|'))
        {
            var split = machineName.Split('|');
            machineName = split[0];
            instanceName = split[1];
        }

        machineName = machineName.Replace(".foo.corp", string.Empty).ToLower();

        var alert = new Alert(data.MetricName, machineName, instanceName, metrics);

        await __processor.ProcessAlert(alert);
        
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
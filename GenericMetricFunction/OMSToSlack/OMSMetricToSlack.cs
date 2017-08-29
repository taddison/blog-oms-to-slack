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
    [FunctionName("OMSMetricToSlack")]
    public static async Task<object> Run([HttpTrigger(WebHookType = "genericJson")]HttpRequestMessage req, TraceWriter log)
    {
        var data = await req.Content.ReadAsAsync<OMSPayload>();

        var metrics = data.SearchResults.Tables[0].Rows.Select(r =>
        {
            return new MetricValue(DateTime.Parse(r[0].ToString()), Double.Parse(r[2].ToString()) * data.ValueMultiplier);
        });

        // Server1|E:
        var computerName = data.SearchResults.Tables[0].Rows[0][1].ToString();
        string instanceName = null;

        if(computerName.Contains('|'))
        {
            var split = computerName.Split('|');
            computerName = split[0];
            instanceName = split[1];
        }

        var alert = new Alert(
            data.Channel
            , data.WarningThreshold
            , data.CriticalThreshold
            , data.LessThanThresholdIsBad
            , data.AlertMessage
            , data.MetricName
            , data.FormatString
            , computerName
            , instanceName
            , data.ObservationThreshold
            , metrics
        );

        AlertProcessor.ProcessAlert(alert);
        
        return req.CreateResponse(HttpStatusCode.OK);
    }
}
using System.Collections.Generic;

namespace OMSToSlack.Models.Configs
{
    public class OverrideAlertNotificationConfig
    {
        public OverrideAlertNotificationConfig(string metricName = null, string machineName = null, List<string> channels = null)
        {
            MetricName = metricName;
            MachineName = machineName;
            Channels = channels;
        }

        public string MetricName { get; set; }
        public string MachineName { get; set; }
        public List<string> Channels { get; set; }
    }
}

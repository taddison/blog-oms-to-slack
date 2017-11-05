using System.Collections.Generic;

namespace OMSToSlack.Models
{
    public class Alert
    {
        public Alert(string metricName, string machineName, string instanceName, IEnumerable<MetricValue> metricValues)
        {
            MetricName = metricName;
            MachineName = machineName;
            InstanceName = instanceName;
            MetricValues = metricValues;
        }

        public string MetricName { get; private set; }
        public string MachineName { get; private set; }
        public string InstanceName { get; private set; }
        public IEnumerable<MetricValue> MetricValues { get; private set; }
    }
}

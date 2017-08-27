using System.Collections.Generic;

namespace OMSToSlack.Models
{
    public class Alert
    {
        public Alert(string defaultChannel, double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , string alertMessage, string metricName, string formatString, string machineName, string instanceName
            , int observationThreshold, IEnumerable<MetricValue> metricValues)
        {
            DefaultChannel = defaultChannel;
            DefaultWarningThreshold = warningThreshold;
            DefaultCriticalThreshold = criticalThreshold;
            DefaultAlertMessage = alertMessage;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            ObservationThreshold = observationThreshold;
            MetricName = metricName;
            FormatString = formatString;
            MachineName = machineName;
            InstanceName = instanceName;
            MetricValues = metricValues;
        }

        public string DefaultChannel { get; private set; }
        public double DefaultWarningThreshold { get; private set; }
        public double DefaultCriticalThreshold { get; private set; }
        public int ObservationThreshold { get; private set; }
        public bool LessThanThresholdIsBad { get; private set; }
        public string DefaultAlertMessage { get; private set; }
        public string MetricName { get; private set; }
        public string FormatString { get; private set; }

        public string MachineName { get; private set; }
        public string InstanceName { get; private set; }
        public IEnumerable<MetricValue> MetricValues { get; private set; }
    }
}

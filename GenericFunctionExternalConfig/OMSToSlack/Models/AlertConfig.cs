namespace OMSToSlack.Models
{
    class AlertConfig
    {
        public AlertConfig(string defaultChannel, double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , string alertMessage, string metricName, string formatString, int observationThreshold, double valueMultiplier)
        {
            DefaultChannel = defaultChannel;
            DefaultWarningThreshold = warningThreshold;
            DefaultCriticalThreshold = criticalThreshold;
            DefaultAlertMessage = alertMessage;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            ObservationThreshold = observationThreshold;
            MetricName = metricName;
            FormatString = formatString;
            ValueMultiplier = valueMultiplier;
        }

        public string DefaultChannel { get; private set; }
        public double DefaultWarningThreshold { get; private set; }
        public double DefaultCriticalThreshold { get; private set; }
        public int ObservationThreshold { get; private set; }
        public bool LessThanThresholdIsBad { get; private set; }
        public string DefaultAlertMessage { get; private set; }
        public string MetricName { get; private set; }
        public string FormatString { get; private set; }
        public double ValueMultiplier { get; set; }

    }
}

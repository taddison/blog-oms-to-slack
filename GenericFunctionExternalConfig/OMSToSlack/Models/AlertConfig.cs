namespace OMSToSlack.Models
{
    class AlertConfig
    {
        public AlertConfig(double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , string alertMessage, string metricName, string formatString, int observationThreshold, double valueMultiplier)
        {
            DefaultWarningThreshold = warningThreshold;
            DefaultCriticalThreshold = criticalThreshold;
            DefaultAlertMessage = alertMessage;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            ObservationThreshold = observationThreshold;
            MetricName = metricName;
            FormatString = formatString;
            ValueMultiplier = valueMultiplier;
        }

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

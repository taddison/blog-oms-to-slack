namespace OMSToSlack.Models.Configs
{
    public class OverrideAlertConfig
    {
        public OverrideAlertConfig(string metricName, string machineName, double? warningThreshold = null
            , double? criticalThreshold = null, int? minimumViolationsToAlert = null)
        {
            MetricName = metricName;
            MachineName = machineName;
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            MinimumViolationsToAlert = minimumViolationsToAlert;
        }

        public string MetricName { get; private set; }
        public string MachineName { get; private set; }
        public double? WarningThreshold { get; private set; }
        public double? CriticalThreshold { get; private set; }
        public int? MinimumViolationsToAlert { get; private set; }
    }
}

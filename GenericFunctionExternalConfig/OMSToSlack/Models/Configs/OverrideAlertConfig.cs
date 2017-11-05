namespace OMSToSlack.Models.Configs
{
    public class OverrideAlertConfig
    {
        public OverrideAlertConfig(double? warningThreshold = null, double? criticalThreshold = null, int? minimumViolationsToAlert = null)
        {
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            MinimumViolationsToAlert = minimumViolationsToAlert;
        }

        public double? WarningThreshold { get; private set; }
        public double? CriticalThreshold { get; private set; }
        public int? MinimumViolationsToAlert { get; private set; }
    }
}

using OMSToSlack.Models.Configs;
using System.Collections.Generic;

namespace OMSToSlack
{
    public static class ConfigHelper
    {
        public static List<DefaultAlertConfig> GetDefaultAlertConfigs()
        {
            // TODO: Unique on metricName
            var configs = new List<DefaultAlertConfig>
            {
                new DefaultAlertConfig("Processor Usage %", 0.35, 0.5, false, 3, 0.01),
                new DefaultAlertConfig("Free Space %", 0.2, 0.1, true, 1, 0.01),
                new DefaultAlertConfig("Free Megabytes", 10000, 5000, true, 1, 1)
            };

            return configs;
        }
    }
}

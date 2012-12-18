using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ProcessExaminator
{
    public static class Config
    {
        public static string OutputDir = "OutputDir";
        public static string PerfNumSamples = "NumberOfSamplesForPerformance";
        public static string PerfTimeBetweenSamples = "TimeBetweenSamplesForPerformance";
        public static string TargetProcessName = "ProcessNameToAttchTo";
        public static string PrintInnerExceptions = "PrintInnerExceptions";
        private static Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static string GetStringValue(string key)
        {
            var element = conf.AppSettings.Settings[key];
            if (element != null)
                return element.Value;
            return null;
        }

        public static int GetIntValue(string key)
        {
            string val = GetStringValue(key) ;
            int retVal = int.MinValue;
            if (val != null && int.TryParse(val, out retVal))
            {
                return retVal;
            }
            return int.MinValue;
        }
    }
}

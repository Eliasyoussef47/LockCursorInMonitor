using Configs;
using Newtonsoft.Json;

namespace LockCursorInMonitor.Configurations
{
    class AppConfigs : ConfigsTools
    {
        public AppConfigs()
        {
            
        }

        [JsonProperty("activated", Required = Required.AllowNull)]
        public bool Activated { get; set; }

        public static AppConfigs GetConfigs()
        {
            return GetConfigs<AppConfigs>();
        }
    }
}

using System.Threading.Tasks;
using UnityEngine;

namespace CloudService
{
    public class StatService : BaseCloudServiceSingleton<StatService>
    {
        private CloudLogger.CloudLoggerSingular Logger;

        public StatService()
        {
            Logger = CloudLogger.Singleton.Get("Stat");
        }

        public async override Task Initialize()
        {
            Logger.Log("initialization");
            Logger.Log("initialization complete");
        }
    }
}

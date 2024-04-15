using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObserverPattern;
using UnityEngine;
using Unity.Services.Economy.Model;
using Unity.Services.Economy;

namespace CloudService
{
    public class EconomyService : BaseCloudServiceSingleton<EconomyService>
    {
        private CloudLogger.CloudLoggerSingular Logger;
        public List<VirtualPurchaseDefinition> purchaseDefinitions;

        public EconomyService()
        {
            Logger = CloudLogger.Singleton.Get("Economy");
        }

        public async override Task Initialize()
        {
#if !DEDICATED_SERVER
            await Unity.Services.Economy.EconomyService.Instance.Configuration.SyncConfigurationAsync();
            Logger.Log("initialization");
            purchaseDefinitions = Unity.Services.Economy.EconomyService.Instance.Configuration.GetVirtualPurchases();
            Debug.Log("purchase definition count: " + purchaseDefinitions.Count);
            isServiceReady.Value = true;
            Logger.Log("initialization complete");
#endif
        }
    }
}

using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using ObserverPattern;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace CloudService
{
    public class StatService : NetworkBehaviour
    {
        private CloudLogger.CloudLoggerSingular Logger;
        public static StatService Singleton;
        private HashSet<string> listOfAllStatName;

        [SerializeField] private List<BaseStatCollector> statCollectors;

        public void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                Logger = CloudLogger.Singleton.Get("Stat");
                for (int i = 0; i < statCollectors.Count; i++)
                    statCollectors[i] = Instantiate(statCollectors[i]);
                listOfAllStatName = PullStatIdFromCollector();
            }
            else
                Destroy(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            FinalizeStat();
        }

        private async Task GetAllPlayerData()
        {
            Logger.Log("getting all the player data rn");
            HashSet<string> listOfStat = PullStatIdFromCollector();
            await GetPlayerData(listOfStat);
            Logger.Log("Finished getting all the player daata");
        }

        private async Task GetPlayerData(HashSet<string> statList)
        {
            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(statList);
                Logger.Log("player Data: " + playerData);
                var data = new Dictionary<string, object>();
                foreach (KeyValuePair<string, Unity.Services.CloudSave.Models.Item> pair in playerData)
                    data.Add(pair.Key, pair.Value.Value);
                await PushPlayerDataToCollector(data);
            }
            catch (Exception r)
            {
                Logger.LogError(r.ToString());
            }
        }

        private HashSet<string> PullStatIdFromCollector()
        {
            Logger.Log("Pulling the stat id from collector");
            HashSet<string> allStatName = new HashSet<string>();
            foreach (var collector in statCollectors)
            {
                foreach (var stat in collector.listOfStats)
                {
                    if (!allStatName.Add(stat.name))
                        Logger.LogError($"Stats called {stat.name} is presented in multiple stat collector");
                }
            }

            return allStatName;
        }

        private async Task PushPlayerDataToCollector(Dictionary<string, object> playerData)
        {
            Logger.Log("Pushing player data to collector");
            Dictionary<string, List<BaseStatCollector.NetworkStat>> stats = new Dictionary<string, List<BaseStatCollector.NetworkStat>>();
            List<Task> uploadToCloud = new List<Task>();
            foreach (var collector in statCollectors)
            {
                var unseenData = collector.PullStatFromStatService(playerData);
                var dict = new Dictionary<string, object>();
                unseenData.ForEach((networkStat) => dict.Add(networkStat.name, networkStat.defaultValue));
                uploadToCloud.Add(SetCloudData(dict));
            }
            await Task.WhenAll(uploadToCloud);
            Logger.Log("Finished Pushing player data to collector");
        }

        private async Task FinalizeStat()
        {
            Logger.Log("Finalizing Stat");
            var dict = new Dictionary<string, object>();
            foreach (var collector in statCollectors)
            {
                foreach (KeyValuePair<string, object> pair in collector.PushStatToStatService())
                {
                    dict.Add(pair.Key, pair.Value);
                }
            }

            await SetCloudData(dict);
        }

        private async Task SetCloudData(Dictionary<string, object> dict)
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(dict);
        }

        public async Task Initialize()
        {
        }
    }
}

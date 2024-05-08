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

        private Dictionary<string, Dictionary<string, object>> allPlayersFullData = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<ulong, string> networkIdMapper = new Dictionary<ulong, string>();

        public Subject<bool> hasOperationFinished = new Subject<bool>(false);

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

        public override void OnNetworkSpawn()
        {
#if DEDICATED_SERVER
            Logger.Log("initialization");
            /* NetworkManager.Singleton.OnClientConnectedCallback += CreateMapper; */
            NetworkManager.Singleton.OnClientDisconnectCallback += (id) => FinalizeStat(id);
            CreateMapper();
            /* GetAllPlayerData(); */
            Logger.Log("initialization complete");
#endif
        }

        private async Task GetAllPlayerData()
        {
#if DEDICATED_SERVER
            Logger.Log("getting all the player data rn");
            HashSet<string> listOfStat = PullStatIdFromCollector();
            List<Task> allTasks = new List<Task>();
            foreach (var player in networkIdMapper.Values)
                allTasks.Add(GetPlayerData(player, listOfStat));
            await Task.WhenAll(allTasks);
            Logger.Log("Finished getting all the player daata");
#endif
        }

        [ClientRpc]
        private void GetPlayerIDClientRpc()
        {
            Logger.Log("My id is: " + Unity.Services.Authentication.AuthenticationService.Instance.PlayerId);
            SetPlayerIDServerRpc(Unity.Services.Authentication.AuthenticationService.Instance.PlayerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerIDServerRpc(NetworkString playerId, ServerRpcParams rpcParams = default)
        {
            Logger.Log("getting the id from server rpc: " + playerId);
#if DEDICATED_SERVER
            Logger.Log("Adding: " + rpcParams.Receive.SenderClientId + ", playerId: " + playerId);
            networkIdMapper.Add(rpcParams.Receive.SenderClientId, playerId);

            if (networkIdMapper.Count >= CloudService.MatchMakingService.Singleton.totalPlayerInCurrentMatch)
                GetAllPlayerData();
            /* GetPlayerData(playerId, listOfAllStatName); */
#endif
            Logger.Log("Finishing the set player id server rpc");
        }

        private async Task GetPlayerData(string playerKey, HashSet<string> statList)
        {
            Logger.Log($"getting player data with id {playerKey}");
            try
            {
                var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(statList,
                    new LoadOptions(new PublicReadAccessClassOptions(playerKey)));
                Logger.Log("player Data: " + playerData);
                var data = new Dictionary<string, object>();
                foreach (KeyValuePair<string, Unity.Services.CloudSave.Models.Item> pair in playerData)
                    data.Add(pair.Key, pair.Value.Value);
                allPlayersFullData.Add(playerKey, data);
                await PushPlayerDataToCollector();
            }
            catch (Exception r)
            {
                Logger.LogError(r.ToString());
            }
        }

        private void CreateMapper()
        {
            if (!IsServer) return;
            Logger.Log("Creating Mapper");
            // Logger.Log("GetPlayerIDCLientRPC playerID: " + playerId);
            GetPlayerIDClientRpc();
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

        private async Task PushPlayerDataToCollector()
        {
#if DEDICATED_SERVER
            Logger.Log("Pushing player data to collector");
            Dictionary<string, List<BaseStatCollector.NetworkStat>> stats = new Dictionary<string, List<BaseStatCollector.NetworkStat>>();
            List<Task> uploadToCloud = new List<Task>();
            foreach (var collector in statCollectors)
            {
                foreach (KeyValuePair<string, Dictionary<string, object>> valuePairs in allPlayersFullData)
                {
                    var unseenData = collector.PullStatFromStatService(valuePairs.Key, valuePairs.Value);
                    var dict = new Dictionary<string, object>();
                    unseenData.ForEach((networkStat) => dict.Add(networkStat.name, networkStat.defaultValue));
                    uploadToCloud.Add(SetCloudStat(valuePairs.Key, dict));
                }
            }
            await Task.WhenAll(uploadToCloud);
            Logger.Log("Finished Pushing player data to collector");
#endif
        }

        private async Task SetCloudStat(string playerId, Dictionary<string, object> stats)
        {
            // Web request must be used to save each player data to cloud save
#if ENABLE_UCS_SERVER 
            Logger.Log("Setting the stat on the cloud");
            var envId = CloudServiceManager.Singleton.environmentID;
            var projectId = CloudServiceManager.Singleton.projectId;
            var payload = JsonUtility.ToJson(stats);
            var token = Unity.Services.Authentication.Server.ServerAuthenticationService.Instance.AccessToken;
            try 
            {
            using (UnityWebRequest www = UnityWebRequest.
                    Post($"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{envId}/players/{playerId}/items", payload, "application/json"))
            {
                www.SetRequestHeader("Authentication", AuthenticationService.Singleton.adminAuth);
                www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogError(www.error, true);
                }
                else
                {
                    Logger.Log("stat save successfully");
                }
            }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
#endif
        }

        private async Task FinalizeStat(ulong networkId)
        {
#if DEDICATED_SERVER
            Logger.Log("Finalizing Stat");
            foreach (var collector in statCollectors)
            {
                await SetCloudStat(networkIdMapper[networkId], collector.PushStatToStatService(networkIdMapper[networkId]));
                await AchievementService.Singleton.UpdateAchievementServerSide(networkIdMapper[networkId]);
            }

            if (CloudService.MatchMakingService.Singleton.totalPlayerInCurrentMatch <= 0)
                hasOperationFinished.Value = true;
#endif
        }

        public async Task Initialize()
        {
        }
    }
}

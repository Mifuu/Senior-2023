using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using ObserverPattern;

namespace CloudService
{
    public class StatService : NetworkBehaviour
    {
        private CloudLogger.CloudLoggerSingular Logger;
        public static StatService Singleton;
        [SerializeField] private List<BaseStatCollector> statCollectors;

#if DEDICATED_SERVER

        private Dictionary<string, Dictionary<string, object>> allPlayersFullData = new Dictionary<string, Dictionary<string, object>>();
        public Dictionary<ulong, string> networkIdMapper;

        public Subject<bool> hasOperationFinished = new Subject<bool>(false);

        public void Awake()
        {
            if (Singleton == null)
            {
                Logger = CloudLogger.Singleton.Get("Stat");
                Singleton = this;
            }
            else
                Destroy(this);
        }

        public async override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += CreateMapper;
            NetworkManager.Singleton.OnClientDisconnectCallback += FinalizeStat;
            await GetAllPlayerData();
        }

        private async Task GetAllPlayerData()
        {
            List<Task> allTasks = new List<Task>();
            foreach (var player in CloudService.MatchMakingService.listOfAllPlayers)
                allTasks.Add(GetPlayerData(player.Id));
            await Task.WhenAll(allTasks);
        }

        [ClientRpc]
        private NetworkString GetPlayerIDClientRpc(ClientRpcParams rpcParams = default)
        {
            if (CloudService.AuthenticationService.Singleton.currentPlayer == null)
                return "";
            return CloudService.AuthenticationService.Singleton.currentPlayer.Id;
        }

        private async Task GetPlayerData(string playerKey, HashSet<string> statList)
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAllAsync(
                new LoadAllOptions(new PublicReadAccessClassOptions(playerKey)));

            var data = new Dictionary<string, object>();
            foreach (KeyValuePair<string, Unity.Services.CloudSave.Models.Item> pair in playerData)
                data.Add(pair.Key, pair.Value.Value);
            allPlayersFullData.Add(playerKey, data);
            await PushPlayerDataToCollector();
        }

        private void CreateMapper(ulong playerId)
        {
            if (!IsServer) return;
            var id = GetPlayerIDClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerId }
                }
            });

            networkIdMapper.Add(playerId, id);
        }

        private HashSet<string> PullStatIdFromCollector()
        {
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
        }

        private async Task SetCloudStat(string playerId, Dictionary<string, object> stats)
        {
            // TODO: How do the server write the player's data?
            await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.SaveAsync(stats,
                    new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
        }

        private async Task FinalizeStat(ulong networkId)
        {
            foreach (var collector in statCollectors)
            {
                await SetCloudStat(networkIdMapper[networkId], collector.PushStatToStatService(networkIdMapper[networkId]));
                await AchievementService.Singleton.UpdateAchievementServerSide();
            }

            if (CloudService.MatchMakingService.totalPlayerInCurrentMatch <= 0)
                hasOperationFinished.Value = true;
        }

#endif

        public async Task Initialize()
        {
            Logger.Log("initialization");
            Logger.Log("initialization complete");
        }
    }
}

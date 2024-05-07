using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ObserverPattern;
using Unity.Services.CloudSave.Models.Data.Player;
using UnityEngine.Networking;

namespace CloudService
{
    public class AchievementService : MonoBehaviour
    {
        public static AchievementService Singleton;
        private CloudLogger.CloudLoggerSingular Logger;

        public List<AchievementClaimable> listOfCompletedAchievement = new List<AchievementClaimable>();
        public AchievementList achievementList;
        public Subject<bool> isServiceReady = new Subject<bool>(false);

        public void Awake()
        {
            if (Singleton == null)
            {
                DontDestroyOnLoad(this);
                Singleton = this;
            }
            else
                Destroy(this);
        }

        public async Task Initialize()
        {
            Logger = CloudLogger.Singleton.Get("Achievement");
#if !DEDICATED_SERVER
            Logger.Log("initialization");
            await FetchCompletedAchievement();
            isServiceReady.Value = true;
            Logger.Log("initialization complete");
#endif 
        }

        public async Task UpdateAchievementServerSide(string playerId)
        {
#if ENABLE_UCS_SERVER
            var playerDatas = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAllAsync(new LoadAllOptions(new PublicReadAccessClassOptions(playerId)));
            var achievementData = await Unity.Services.CloudSave.CloudSaveService
                .Instance.Data.Player.LoadAsync(new HashSet<string>() { "achievement" }, 
                    new LoadOptions(new PublicReadAccessClassOptions(playerId)));
            List<AchievementClaimable> claimables;

            if (achievementData.TryGetValue("achievement", out var achievement))
                claimables = achievement.Value.GetAs<List<AchievementClaimable>>();
            else
                claimables = new List<AchievementClaimable>();

            foreach (var ac in achievementList.allAchievement)
            {
                if (!ac.CheckAchievementServerSide(playerDatas, out var achievementId)) continue;
                claimables.Add(new AchievementClaimable(achievementId, false));
            }

           // Are you fucking kidding me?
            var envId = CloudServiceManager.Singleton.environmentID;
            var projectId = CloudServiceManager.Singleton.projectId;
            var payload = JsonUtility.ToJson(new Dictionary<string, object>() {{ "achievement", claimables }});
            var token = Unity.Services.Authentication.Server.ServerAuthenticationService.Instance.AccessToken;
            using (UnityWebRequest www = UnityWebRequest.
                    Post($"https://services.api.unity.com/cloud-save/v1/data/projects/{projectId}/environments/{envId}/players/{playerId}/items", payload, "application/json"))
            {
                www.SetRequestHeader("Authentication", $"Bearer {token}");
                www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Logger.LogError(www.error, true);
                }
                else
                {
                    Logger.Log("achievement save successfully");
                }
            }
            /* await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { { "achievement", claimables } }); */
#endif
        }

        public async Task FetchCompletedAchievement()
        {
            var playerData = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAllAsync();
            if (playerData.TryGetValue("achievement", out var achievement))
                listOfCompletedAchievement = achievement.Value.GetAs<List<AchievementClaimable>>();
            achievementList.allAchievement.ForEach((a) => a.CheckAchievementClientSide(listOfCompletedAchievement));
        }
    }
}

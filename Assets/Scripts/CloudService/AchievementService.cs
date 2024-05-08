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
    public class AchievementService : BaseCloudServiceSingleton<AchievementService>
    {
        private CloudLogger.CloudLoggerSingular Logger;

        public List<AchievementClaimable> listOfCompletedAchievement = new List<AchievementClaimable>();
        public AchievementList achievementList;

        public override async Task Initialize()
        {
            Logger = CloudLogger.Singleton.Get("Achievement");
            Logger.Log("initialization");
            await FetchCompletedAchievement();
            isServiceReady.Value = true;
            Logger.Log("initialization complete");
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

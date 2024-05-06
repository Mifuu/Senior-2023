using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudService
{
    public class AchievementService : BaseCloudServiceSingleton<AchievementService>
    {
        private CloudLogger.CloudLoggerSingular Logger;
        /* public List<Achievement> achievementsList; */

        public List<AchievementClaimable> listOfCompletedAchievement;
        public List<BaseAchievement> listOfAllAchievement;

        public AchievementService()
        {
            Logger = CloudLogger.Singleton.Get("Achievement");
            /* achievementsList = new List<Achievement>(); */
        }

        public async override Task Initialize()
        {
#if !DEDICATED_SERVER
            Logger.Log("initialization");
            /* await LoadAllAchievement(); */
            Logger.Log("initialization complete");
#endif 
        }

        /* public async Task LoadAllAchievement() */
        /* { */
        /*     Logger.Log("Loading all achievement"); */
        /*     var achievementData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync("achievement"); */
        /*     foreach (var achievement in achievementData) */
        /*     { */
        /*         var acObj = ConvertDictToAchievementObj(achievement.Value.Value.GetAs<Dictionary<string, string>>()); */
        /*         achievementsList.Add(acObj); */
        /*     } */
        /*     Logger.Log("Finished loading achievement"); */
        /*     isServiceReady.Value = true; */
        /* } */

        /* public Achievement ConvertDictToAchievementObj(Dictionary<string, string> dict) */
        /* { */
        /*     foreach (KeyValuePair<string, string> entry in dict) */
        /*         Debug.Log($"{entry.Key}={entry.Value}"); */

        /*     var achieve = new Achievement(); */
        /*     dict.TryGetValue("id", out achieve.id); */
        /*     dict.TryGetValue("name", out achieve.name); */
        /*     dict.TryGetValue("description", out achieve.description); */
        /*     dict.TryGetValue("icon", out achieve.icon); */

        /*     dict.TryGetValue("rewardCurrencyID", out achieve.rewardCurrencyID); */
        /*     dict.TryGetValue("statFieldName", out achieve.statFieldName); */
        /*     dict.TryGetValue("condition", out achieve.condition); */
        /*     dict.TryGetValue("conditionValue", out achieve.conditionValue); */
        /*     dict.TryGetValue("category", out achieve.category); */

        /*     if (dict.TryGetValue("rewardAmount", out var rewardAmount)) */
        /*     { */
        /*         if (Int32.TryParse(rewardAmount, out var rewardAmountInt)) */
        /*             achieve.rewardAmount = rewardAmountInt; */
        /*     } */
        /*     else */
        /*         achieve.rewardAmount = 100; */

        /*     return achieve; */
        /* } */

        // TODO: This should be a function that writes to one player achievement
        // How do i write to the data of just one player?
        public async Task UpdateAchievementServerSide()
        {
            var playerDatas = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAllAsync();
            var achievementData = await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>() { "achievement" });
            List<AchievementClaimable> claimables;

            if (achievementData.TryGetValue("achievement", out var achievement))
                claimables = achievement.Value.GetAs<List<AchievementClaimable>>();
            else
                claimables = new List<AchievementClaimable>();

            foreach (var ac in listOfAllAchievement)
            {
                if (!ac.CheckAchievementServerSide(playerDatas, out var achievementId)) continue;
                claimables.Add(new AchievementClaimable(achievementId, false));
            }

            await Unity.Services.CloudSave.CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { { "achievement", claimables } });
        }

        public void FetchCompletedAchievement()
        {

        }
    }

    /* public struct Achievement */
    /* { */
    /*     public string id; */
    /*     public string name; */
    /*     public string description; */
    /*     public string icon; */

    /*     public int rewardAmount; */
    /*     public string rewardCurrencyID; */

    /*     public string statFieldName; */
    /*     public string condition; */
    /*     public string conditionValue; */

    /*     public string category; */
    /* } */
}

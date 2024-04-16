using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;
using System;
using System.Collections.Generic;

namespace CloudService
{
    public class AchievementService : BaseCloudServiceSingleton<AchievementService>
    {
        private CloudLogger.CloudLoggerSingular Logger;
        public List<Achievement> achievementsList;

        public AchievementService()
        {
            Logger = CloudLogger.Singleton.Get("Achievement");
            achievementsList = new List<Achievement>();
        }

        public async override Task Initialize()
        {
#if !DEDICATED_SERVER
            Logger.Log("initialization");
            await LoadAllAchievement();
            Logger.Log("initialization complete");
#endif 
        }

        public async Task LoadAllAchievement()
        {
            Logger.Log("Loading all achievement");
            var achievementData = await CloudSaveService.Instance.Data.Custom.LoadAllAsync("achievement");
            foreach (var achievement in achievementData)
            {
                var acObj = ConvertDictToAchievementObj(achievement.Value.Value.GetAs<Dictionary<string, string>>());
                achievementsList.Add(acObj);
            }
            Logger.Log("Finished loading achievement");
            isServiceReady.Value = true;
        }

        public Achievement ConvertDictToAchievementObj(Dictionary<string, string> dict)
        {
            foreach (KeyValuePair<string, string> entry in dict)
                Debug.Log($"{entry.Key}={entry.Value}");

            var achieve = new Achievement();
            dict.TryGetValue("id", out achieve.id);
            dict.TryGetValue("name", out achieve.name);
            dict.TryGetValue("description", out achieve.description);
            dict.TryGetValue("icon", out achieve.icon);

            dict.TryGetValue("rewardCurrencyID", out achieve.rewardCurrencyID);
            dict.TryGetValue("statFieldName", out achieve.statFieldName);
            dict.TryGetValue("condition", out achieve.condition);
            dict.TryGetValue("conditionValue", out achieve.conditionValue);
            dict.TryGetValue("category", out achieve.category);

            if (dict.TryGetValue("rewardAmount", out var rewardAmount))
            {
                if (Int32.TryParse(rewardAmount, out var rewardAmountInt))
                    achieve.rewardAmount = rewardAmountInt;
            }
            else
                achieve.rewardAmount = 100;

            return achieve;
        }
    }

    public struct Achievement
    {
        public string id;
        public string name;
        public string description;
        public string icon;

        public int rewardAmount;
        public string rewardCurrencyID;

        public string statFieldName;
        public string condition;
        public string conditionValue;
        
        public string category;
    }
}

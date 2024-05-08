using System;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementCheckAmount", menuName = "Achievement/Achievement/Check Amount")]
public class AchievementCheckAmount : BaseAchievement
{
    [Header("Stat Requirement")]
    [SerializeField] private string statName;
    [SerializeField] private float amount;
    [SerializeField] private Condition condition;

    [Serializable]
    [System.Flags]
    public enum Condition
    {
        Equal = 1,
        Morethan = 2,
        Lessthan = 4,
    }

    public override bool CheckAchievementServerSide(Dictionary<string, Item> dict, out string achievementId)
    {
        achievementId = id;

        if (!dict.TryGetValue(statName, out Item value))
        {
            Debug.Log("Can not find the stat name: " + statName);
            return false;
        }
        
        float currentValue = value.Value.GetAs<float>();
        if (condition.HasFlag(Condition.Equal))
        {
            if (currentValue != amount) return false;
        }

        if (condition.HasFlag(Condition.Lessthan))
        {
            if (currentValue >= amount) return false;
        }

        if (condition.HasFlag(Condition.Morethan))
        {
            if (currentValue <= amount) return false;
        }

        return true;
    }
}

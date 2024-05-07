using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;

public abstract class BaseAchievement : ScriptableObject
{
    [SerializeField] public string id;
    public abstract bool CheckAchievementServerSide(Dictionary<string, Unity.Services.CloudSave.Models.Item> dict, out string achievementId);

    // Frontend
    [Header("Description")]
    [SerializeField] public new string name;
    [SerializeField] public string description;
    [SerializeField] public Image icon;

    [Header("Reward")]
    [SerializeField] public int rewardAmount;
    [SerializeField] public string rewardCurrencyId;

    [Header("Display")]
    [SerializeField] public List<AchievementDisplay> displays;

    [HideInInspector] public AchievementStatus status = AchievementStatus.Ongoing;

    public void CheckAchievementClientSide(List<AchievementClaimable> unlocked)
    {
        foreach (var able in unlocked)
        {
            if (able.id == id)
            {
                if (able.claimed)
                    status = AchievementStatus.Claimed;
                else
                    status = AchievementStatus.Claimable;
                return;
            }
        }
    }

    public async Task ClaimAchievement()
    {
        if (status != AchievementStatus.Claimable) return;
        await Unity.Services.Economy.EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(rewardCurrencyId, rewardAmount);
        CloudService.AchievementService.Singleton.listOfCompletedAchievement.Select((claimable) =>
        {
            if (claimable.id == id)
                claimable.claimed = true;
            return claimable;
        });
        status = AchievementStatus.Claimed;
        await CloudService.AchievementService.Singleton.FetchCompletedAchievement();
    }
}

public class AchievementDisplay
{
    public string parameterDisplayString;
    public string parameterId;
    public string goalValue;
}

public struct AchievementClaimable
{
    public string id;
    public bool claimed;

    public AchievementClaimable(string id, bool claimed)
    {
        this.id = id;
        this.claimed = claimed;
    }
}

public enum AchievementStatus
{
    Ongoing,
    Claimable,
    Claimed
}

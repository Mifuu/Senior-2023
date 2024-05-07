using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AchievementItem : MonoBehaviour
{
    [SerializeField] private Image imageGameObject;
    [SerializeField] private TextMeshProUGUI titleGameObj;
    [SerializeField] private TextMeshProUGUI descriptionGameObj;
    [SerializeField] private Button claimButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private BaseAchievement achievement;

    public void SetDetail(BaseAchievement achievement)
    {
        this.achievement = achievement;
        Rerender();
    }

    public void Rerender()
    {
        this.titleGameObj.text = achievement.name;
        this.descriptionGameObj.text = achievement.description;
        this.imageGameObject = achievement.icon;
        this.claimButton.onClick.RemoveAllListeners();
        this.claimButton.onClick.AddListener(async () => await this.achievement.ClaimAchievement());
        this.claimButton.interactable = false;
        this.buttonText.text = achievement.rewardAmount + " "  + achievement.rewardCurrencyId;

        if (this.achievement.status == AchievementStatus.Claimable)
            this.claimButton.interactable = true;
        if (this.achievement.status == AchievementStatus.Claimed)
            this.buttonText.text = "Claimed";
    }

    /* private CloudService.Achievement achievement; */

    /* public void SetDetail(CloudService.Achievement achievement) */
    /* { */
    /*     this.achievement = achievement; */
    /*     Rerender(); */
    /* } */

    /* public void Rerender() */
    /* { */
    /*     titleObj.text = achievement.name; */
    /*     descriptionObj.text = achievement.description; */
    /* } */
}

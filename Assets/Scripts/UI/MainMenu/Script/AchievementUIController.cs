using UnityEngine;
using UnityEngine.UI;

public class AchievementUIController : MonoBehaviour
{
    [SerializeField] private AchievementItem achievementGridObject;
    [SerializeField] private Transform achievementGridLayoutParent;
    [SerializeField] private Button backButton;

    /* public void Awake() */
    /* { */
    /*     CloudService.AchievementService.Singleton.isServiceReady.OnValueChanged += PopulateAchievementGrid; */
/* #if !DEDICATED_SERVER */
    /*     backButton.onClick.AddListener(() => MainMenuUIController.Singleton.menuState.Value = MainMenuUIController.MainMenuState.Main); */
/* #endif */
    /* } */

    /* public void OnDestroy() */
    /* { */
    /*     CloudService.AchievementService.Singleton.isServiceReady.OnValueChanged -= PopulateAchievementGrid; */
/* #if !DEDICATED_SERVER */
    /*     backButton.onClick.RemoveAllListeners(); */
/* #endif */
    /* } */

    /* public void PopulateAchievementGrid(bool prev, bool current) */
    /* { */
    /*     if (!current) return; */
    /*     foreach (var achievement in CloudService.AchievementService.Singleton.achievementsList) */
    /*     { */
    /*         var acObj = Instantiate(achievementGridObject); */
    /*         acObj.transform.SetParent(achievementGridLayoutParent); */
    /*         acObj.transform.localScale = new Vector3(1, 1, 1); */
    /*         acObj.SetDetail(achievement); */
    /*     } */
    /* } */
}

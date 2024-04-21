using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SettingUIController : MonoBehaviour
{
    [Serializable]
    public struct SliderSettingGroup
    {
        [SerializeField] public Slider slider;
        [SerializeField] public TextMeshProUGUI title;
        [SerializeField] public TextMeshProUGUI value;
    }

    [SerializeField] private Button backButton;
    [SerializeField] private SliderSettingGroup volumnSetting;
    [SerializeField] private SliderSettingGroup sensitivitySetting;

    public void Awake()
    {
#if !DEDICATED_SERVER
        backButton.onClick.AddListener(() => MainMenuUIController.Singleton.menuState.Value = MainMenuUIController.MainMenuState.Main);
        volumnSetting.slider.onValueChanged.AddListener(AdjustVolumn);
        sensitivitySetting.slider.onValueChanged.AddListener(AdjustSensitivity);
        volumnSetting.slider.value = 30f;
        sensitivitySetting.slider.value = 30f;
        volumnSetting.value.text = "30";
        sensitivitySetting.value.text = "30";
#endif
    }

    public void OnDestroy()
    {
#if !DEDICATED_SERVER
        backButton.onClick.RemoveAllListeners();
#endif
    }

    private void AdjustVolumn(float volumn)
    {
        PlayerSettingManager.volumn = volumn;
        volumnSetting.value.text = volumn.ToString();
    }

    private void AdjustSensitivity(float sens)
    {
        PlayerSettingManager.sensitivity = sens;
        sensitivitySetting.value.text = sens.ToString();
    }
}

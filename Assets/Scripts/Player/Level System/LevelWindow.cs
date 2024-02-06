using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{
    private LevelSystem levelSystem;

    private TextMeshProUGUI levelText;
    private Image expBarImage;

    private void Awake()
    {
        // get levelText component
        Transform levelTextTransform = transform.Find("levelText");
        levelText = levelTextTransform.GetComponent<TextMeshProUGUI>();

        // get exp bar component
        expBarImage = transform.Find("ExpBar/bar")?.GetComponent<Image>();

        // get a testing button for adding exp by 50
        transform.Find("Exp50").GetComponent<Button>().onClick.AddListener(() => levelSystem.AddExp(50));
    }

    private void SetExpBar(float expNormalized)
    {
        Debug.Log("Setting Exp Bar: " + expNormalized);
        expBarImage.fillAmount = expNormalized;
    }

    private void SetLevelNumber(int Lv)
    {
        Debug.Log("Setting Level Number: " + Lv);
        levelText.text = "Level " + Lv;
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        SetLevelNumber(levelSystem.GetLevel());
        SetExpBar(levelSystem.GetExpNormalized());

        levelSystem.OnExpChange += LevelSystem_OnExpChanged;
        levelSystem.OnLevelChange += LevelSystem_OnLevelChanged;
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        SetLevelNumber(levelSystem.GetLevel());
    }

    private void LevelSystem_OnExpChanged(object sender, System.EventArgs e)
    {
        SetExpBar((float)levelSystem.GetExpNormalized());
    }
}

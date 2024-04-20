using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class GameplayUIBossHP : MonoBehaviour
{
    public static GameplayUIBossHP instance;

    private NetworkVariable<float> reportedHealth;
    private float maxHealth;
    private string bossName;

    public Slider slider;
    public TMP_Text nameText;

    void Awake()
    {
        instance = this;

        CloseHealthBar();
    }

    public void OpenHealthBar(NetworkVariable<float> reportedHealth, float maxHealth, string name)
    {
        if (this.reportedHealth != null)
            this.reportedHealth.OnValueChanged -= OnHealthChanged;

        slider.gameObject.SetActive(true);
        nameText.gameObject.SetActive(true);
        this.reportedHealth = reportedHealth;
        this.maxHealth = maxHealth;
        this.bossName = name;

        reportedHealth.OnValueChanged += OnHealthChanged;

        slider.maxValue = maxHealth;
        slider.value = reportedHealth.Value;
        nameText.text = name;
    }

    public void CloseHealthBar()
    {
        if (this.reportedHealth != null)
            this.reportedHealth.OnValueChanged -= OnHealthChanged;

        slider.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
    }

    private void OnHealthChanged(float prev, float current)
    {
        slider.value = reportedHealth.Value;
    }
}

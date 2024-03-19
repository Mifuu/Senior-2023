using Unity.Netcode;
using System;
using UnityEngine;
using ObserverPattern;
using System.Collections.Generic;

public class GaugeSystem : NetworkBehaviour
{
    public static GaugeSystem Singleton { get; private set; }
    [SerializeField] private List<GaugeLevel> listOfGaugeLevel;

    private const int deteriorationRate = 1; // 1 gauge drop per one FixedDeltaTime
    private int currentGaugeLevelPointer = 0;
    private int currentGaugeNumber = 0;
    private int maximumGaugeValue;
    private int currentGaugeRange;

    public Subject<bool> isGaugeVisible = new Subject<bool>(false); // Used if the gauge number become 0
    public event Action<GaugeSetDetail> OnGaugeLevelChanged; // Used when the gauge level is changed
    public event Action<string> OnNewWordQueued; // Used when the new word is queued;

    public void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
    }

    public void Start()
    {
        if (listOfGaugeLevel.Count == 0)
        {
            Debug.LogError("There are no Gauge Level Setup");
            enabled = false;
            return;
        }

        maximumGaugeValue = listOfGaugeLevel[listOfGaugeLevel.Count - 1].upperBound;
        var currentLevel = listOfGaugeLevel[currentGaugeLevelPointer];
        currentGaugeRange = currentLevel.upperBound;

        // Debug Only
        // OnGaugeLevelChanged += DebugLevelChange;
        // isGaugeVisible.OnValueChanged += DebugVisibleChange;
        // OnNewWordQueued += Debug.Log;
    }

    public void FixedUpdate()
    {
        currentGaugeNumber = Math.Clamp(currentGaugeNumber -= deteriorationRate, 0, maximumGaugeValue);
        AdjustGauge();
    }

    #region Debugging

    public void DebugLevelChange(GaugeSetDetail setDetail)
    {
        Debug.LogWarning("----------------------");
        Debug.LogWarning("Name: " + setDetail.title);
        Debug.LogWarning("Ratio: " + setDetail.ratio);
        Debug.LogWarning("Deterioration Rate: " + setDetail.deteriorationRate);
        Debug.LogWarning("Color: " + setDetail.color);
        Debug.LogWarning("----------------------");
    }

    public void DebugVisibleChange(bool _, bool current)
    {
        Debug.LogWarning("Gauge Visible: " + current);
    }

    #endregion

    // Gauge Number could also be negative in case of player getting hit
    public float AddGauge(string newWord, int gaugeNumberAdded)
    {
        OnNewWordQueued?.Invoke(newWord);
        currentGaugeNumber = Math.Clamp(currentGaugeNumber + gaugeNumberAdded, 0, maximumGaugeValue);
        if (!AdjustGauge()) RecalculateAndEmitGaugeSetDetail();
        return listOfGaugeLevel[currentGaugeLevelPointer].expMultiplier;
    }

    private void SetVisibilty(bool value)
    {
        if (isGaugeVisible.Value && !value)
            isGaugeVisible.Value = false;
        else if (!isGaugeVisible.Value && value)
            isGaugeVisible.Value = true;
    }

    private void RecalculateAndEmitGaugeSetDetail()
    {
        var currentLevel = listOfGaugeLevel[currentGaugeLevelPointer];
        currentGaugeRange = currentGaugeLevelPointer == 0 ? listOfGaugeLevel[0].upperBound
                : (listOfGaugeLevel[currentGaugeLevelPointer].upperBound - listOfGaugeLevel[currentGaugeLevelPointer - 1].upperBound);
        float ratio = ((float)currentGaugeNumber - (currentGaugeLevelPointer == 0 ? 0f :
                    (float)listOfGaugeLevel[currentGaugeLevelPointer - 1].upperBound)) / (float)currentGaugeRange;
        GaugeSetDetail gaugeSetDetail = new GaugeSetDetail(currentLevel.name,
                ratio, (float)deteriorationRate / (float)currentGaugeRange, currentLevel.color);
        OnGaugeLevelChanged?.Invoke(gaugeSetDetail);
    }

    private bool AdjustGauge()
    {
        // Debug.Log("Current Gauge Level: " + currentGaugeNumber);
        SetVisibilty(currentGaugeNumber > 0);

        var levelChanged = false;
        if (currentGaugeLevelPointer >= 0 && currentGaugeLevelPointer < listOfGaugeLevel.Count - 1)
        {
            if (currentGaugeNumber > listOfGaugeLevel[currentGaugeLevelPointer].upperBound)
            {
                currentGaugeLevelPointer++;
                levelChanged = true;
            }
        }

        if (currentGaugeLevelPointer < listOfGaugeLevel.Count && currentGaugeLevelPointer > 0)
        {
            if (currentGaugeNumber < listOfGaugeLevel[currentGaugeLevelPointer - 1].upperBound)
            {
                currentGaugeLevelPointer--;
                levelChanged = true;
            }
        }

        if (levelChanged) RecalculateAndEmitGaugeSetDetail();
        return levelChanged;
    }

    [ContextMenu("Add 800 Gauge Number")]
    private void TestAddGauge() => AddGauge("Test", 800);

    [ContextMenu("Add 1500 Gauge Number")]
    private void TestAddGauge2() => AddGauge("Test", 1500);

    [ContextMenu("Add 3000 Gauge Number")]
    private void TestAddGauge3() => AddGauge("Test", 3000);
}

[Serializable]
public struct GaugeLevel
{
    [SerializeField] public string name;
    [SerializeField] public int upperBound;
    [SerializeField] public float expMultiplier;
    [SerializeField] public Color color;
}

public struct GaugeSetDetail
{
    public string title;
    public float ratio;
    public float deteriorationRate;
    public Color color;

    public GaugeSetDetail(string title, float ratio, float deteriorationRate, Color color)
    {
        this.title = title;
        this.ratio = ratio;
        this.deteriorationRate = deteriorationRate;
        this.color = color;
    }
}

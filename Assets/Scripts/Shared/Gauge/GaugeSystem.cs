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
        /* OnGaugeLevelChanged += DebugLevelChange; */
        /* isGaugeVisible.OnValueChanged += DebugVisibleChange; */
        /* OnNewWordQueued += Debug.Log; */
    }

    public void FixedUpdate()
    {
        currentGaugeNumber = Math.Clamp(currentGaugeNumber -= deteriorationRate, 0, maximumGaugeValue);
        // Debug.Log("Current gauge: " + currentGaugeNumber);
        AdjustGauge(false);
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
    [ClientRpc]
    public void AddGaugeClientRpc(ulong playerId, string newWord, int gaugeNumberAdded, ClientRpcParams clientRpcParams = default)
    {
        OnNewWordQueued?.Invoke(newWord);
        currentGaugeNumber = Math.Clamp(currentGaugeNumber + gaugeNumberAdded, 0, maximumGaugeValue);
        AdjustGauge(true);
        // AddExpServerRpc(playerId, gaugeNumberAdded * GetCurrentGaugeLevelMultiplier());
        float value = gaugeNumberAdded * GetCurrentGaugeLevelMultiplier();
        
        if (NetworkManager.Singleton.LocalClientId == playerId && PlayerManager.thisClient.TryGetComponent<PlayerLevel>(out var level))
            level.AddExp(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddExpServerRpc(ulong playerId, float value)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var playerClient))
        {
            if (playerClient.PlayerObject.TryGetComponent<PlayerLevel>(out var level))
                level.AddExp(value);
        }
    }

    public float GetCurrentGaugeLevelMultiplier() => listOfGaugeLevel[currentGaugeLevelPointer].expMultiplier;

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
        float ratio = Math.Clamp(((float)currentGaugeNumber - (currentGaugeLevelPointer == 0 ? 0f :
                    (float)listOfGaugeLevel[currentGaugeLevelPointer - 1].upperBound)) / (float)currentGaugeRange, 0f, 1f);
        GaugeSetDetail gaugeSetDetail = new GaugeSetDetail(currentLevel.name,
                ratio, (float)deteriorationRate / (float)currentGaugeRange, currentLevel.color);
        OnGaugeLevelChanged?.Invoke(gaugeSetDetail);
    }

    private void AdjustGauge(bool forceEmitEvent)
    {
        // Debug.Log("Current Gauge Level: " + currentGaugeNumber);
        SetVisibilty(currentGaugeNumber > 0);

        var levelChanged = false;
        if (currentGaugeLevelPointer >= 0 && currentGaugeLevelPointer < listOfGaugeLevel.Count - 1)
        {
            while (currentGaugeLevelPointer < listOfGaugeLevel.Count && currentGaugeNumber > listOfGaugeLevel[currentGaugeLevelPointer].upperBound)
            {
                currentGaugeLevelPointer++;
                levelChanged = true;
            }
        }

        if (currentGaugeLevelPointer < listOfGaugeLevel.Count && currentGaugeLevelPointer > 0)
        {
            while (currentGaugeLevelPointer > 0 && currentGaugeNumber < listOfGaugeLevel[currentGaugeLevelPointer - 1].upperBound)
            {
                currentGaugeLevelPointer--;
                levelChanged = true;
            }
        }

        if (levelChanged || forceEmitEvent) RecalculateAndEmitGaugeSetDetail();
    }

    #region Testing Area

    /* [ContextMenu("Add 80 Gauge Number")] */
    /* private void TestAddGauge() => AddGauge("Test", 80); */

    /* [ContextMenu("Add 300 Gauge Number")] */
    /* private void TestAddGauge2() => AddGauge("Test", 300); */

    /* [ContextMenu("Add 1500 Gauge Number")] */
    /* private void TestAddGauge3() => AddGauge("Test", 1500); */

    /* [ContextMenu("Delete 80 Gauge Number")] */
    /* private void TestAddGauge4() => AddGauge("Test", -80); */

    /* [ContextMenu("Delete 300 Gauge Number")] */
    /* private void TestAddGauge5() => AddGauge("Test", -300); */

    /* [ContextMenu("Delete 1500 Gauge Number")] */
    /* private void TestAddGauge6() => AddGauge("Test", -1500); */

    #endregion
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

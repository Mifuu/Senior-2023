using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class PlayerStat : NetworkBehaviour
{
    #region Stats Enum

    public enum StatsEnum { ATK, DEF, CritDMG, CritRate, EDMG, ERES, ALL }
    private List<StatsEnum> StatsGroup1 = new List<StatsEnum>()
    {
        StatsEnum.ATK,
        StatsEnum.DEF,
        StatsEnum.CritDMG,
        StatsEnum.CritRate
    };

    #endregion

    #region Stats Default Value

    [Header("Default Stats")]
    [SerializeField] private float defaultATK = 1.0f;
    [SerializeField] private float defaultDEF = 1.0f;
    [SerializeField] private float defaultCritDMG = 1.0f;
    [SerializeField] private float defaultCritRate = 1.0f;
    [SerializeField] private float defaultElementalDMGBonus = 1.0f;
    [SerializeField] private float defaultElementalRES = 1.0f;

    #endregion

    #region Stats Internal

    private NetworkList<float> BaseStat;
    private NetworkList<float> ElementalDMGBonus;
    private NetworkList<float> ElementalRES;

    #endregion

    #region Stats

    public float BaseATK { get => BaseStat[(int)StatsEnum.ATK]; set => BaseStat[(int)StatsEnum.ATK] = value; }
    public float BaseDEF { get => BaseStat[(int)StatsEnum.DEF]; set => BaseStat[(int)StatsEnum.DEF] = value; }
    public float CritDMG { get => BaseStat[(int)StatsEnum.CritDMG]; set => BaseStat[(int)StatsEnum.CritDMG] = value; }
    public float CritRate { get => BaseStat[(int)StatsEnum.CritRate]; set => BaseStat[(int)StatsEnum.CritRate] = value; }

    public float GetElementDMGBonus(ElementalType element) => ElementalDMGBonus[(int)element];
    public float GetElementRES(ElementalType element) => ElementalRES[(int)element];
    public float SetElementDMGBonud(ElementalType element, float value) => ElementalDMGBonus[(int)element] = value;
    public float SetElementRES(ElementalType element, float value) => ElementalRES[(int)element] = value;

    #endregion

    #region Stats Upgrade

    [Serializable]
    private struct StatUpgradeDetail
    {
        public enum MethodEnums { Additive, Multiplicative, Overwrite }
        [SerializeField] public MethodEnums Method;
        [SerializeField] public List<StatsEnum> Stats;
        [SerializeField] public float Amount;
        [SerializeField] public int LevelLowerBoundInclusive;
        [SerializeField] public int LevelUpperBoundExclusive;
    }

    [Header("Stats Automatic Upgrade Details")]
    [SerializeField] private List<StatUpgradeDetail> UpgradeDetail = new List<StatUpgradeDetail>();

    #endregion

    // NOTE: In Level Changed - Stats Upgrade Event, 
    // OnStatsChanged will be called ONCE even if multiple stats are changed
    public event Action OnStatsChanged;
    private PlayerLevel playerLevel;
    private bool canEmitOnStatChanged = true;
    private bool isStatsReady = false;

    public void Awake()
    {
        BaseStat = new NetworkList<float>(new float[StatsGroup1.Count]);
        ElementalRES = new NetworkList<float>(new float[Enum.GetNames(typeof(ElementalType)).Length]);
        ElementalDMGBonus = new NetworkList<float>(new float[Enum.GetNames(typeof(ElementalType)).Length]);
        playerLevel = GetComponent<PlayerLevel>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer || isStatsReady) return;

        BaseStat[(int)StatsEnum.ATK] = defaultATK;
        BaseStat[(int)StatsEnum.DEF] = defaultDEF;
        BaseStat[(int)StatsEnum.CritRate] = defaultCritRate;
        BaseStat[(int)StatsEnum.CritDMG] = defaultCritDMG;

        foreach (ElementalType elementalType in Enum.GetValues(typeof(ElementalType)))
        {
            ElementalDMGBonus[(int)elementalType] = defaultElementalDMGBonus;
            ElementalRES[(int)elementalType] = defaultElementalRES;
        }

        playerLevel.levelSystem.OnLevelChange += Temp_OnLevelChanged;
        BaseStat.OnListChanged += OnStatsChangedAdapter;
        ElementalRES.OnListChanged += OnStatsChangedAdapter;
        ElementalDMGBonus.OnListChanged += OnStatsChangedAdapter;

        ServerSendStatsReadyClientRpc(true);
    }

    public override void OnNetworkDespawn()
    {
        playerLevel.levelSystem.OnLevelChange -= Temp_OnLevelChanged;
        BaseStat.OnListChanged -= OnStatsChangedAdapter;
        ElementalRES.OnListChanged -= OnStatsChangedAdapter;
        ElementalDMGBonus.OnListChanged -= OnStatsChangedAdapter;
    }

    public override void OnDestroy()
    {
        ElementalDMGBonus?.Dispose();
        ElementalRES?.Dispose();
        BaseStat?.Dispose();
    }

    private void Temp_OnLevelChanged(object o, System.EventArgs e)
    {
        Debug.LogWarning("Stats System is using temporary OnLevelChanged EventHandler");
        OnLevelChanged(0, GetComponent<PlayerLevel>().levelSystem.GetLevel());
    }

    // Used to Adapting/Changing stats when player's level changed
    private void OnLevelChanged(int prev, int current)
    {
        if (!IsServer) return;
        UpgradeStatsOnLevelChanged(current);
    }

    // Level Upgrade does not run often, so iterating through a list should be performant enough
    private void UpgradeStatsOnLevelChanged(float newLevel)
    {
        canEmitOnStatChanged = false;

        // Writing O(n2) code like a pro
        foreach (StatUpgradeDetail upgradeDetail in UpgradeDetail)
        {
            if (!(newLevel >= upgradeDetail.LevelLowerBoundInclusive && newLevel < upgradeDetail.LevelUpperBoundExclusive)) continue;
            foreach (StatsEnum statsEnum in upgradeDetail.Stats)
            {
                if (StatsGroup1.Contains(statsEnum) || statsEnum == StatsEnum.ALL)
                {
                    switch (upgradeDetail.Method)
                    {
                        case StatUpgradeDetail.MethodEnums.Additive:
                            BaseStat[(int)statsEnum] += upgradeDetail.Amount;
                            break;
                        case StatUpgradeDetail.MethodEnums.Multiplicative:
                            BaseStat[(int)statsEnum] *= upgradeDetail.Amount;
                            break;
                        case StatUpgradeDetail.MethodEnums.Overwrite:
                            BaseStat[(int)statsEnum] = upgradeDetail.Amount;
                            break;
                    }
                }

                if (statsEnum == StatsEnum.ALL || statsEnum == StatsEnum.EDMG)
                {
                    // WRITING O(N3) CODE LIKE A PRO
                    for (int i = 0; i < ElementalDMGBonus.Count; i++)
                    {
                        switch (upgradeDetail.Method)
                        {
                            case StatUpgradeDetail.MethodEnums.Additive:
                                ElementalDMGBonus[i] += upgradeDetail.Amount;
                                break;
                            case StatUpgradeDetail.MethodEnums.Multiplicative:
                                ElementalDMGBonus[i] *= upgradeDetail.Amount;
                                break;
                            case StatUpgradeDetail.MethodEnums.Overwrite:
                                ElementalDMGBonus[i] = upgradeDetail.Amount;
                                break;
                        }
                    }
                }

                if (statsEnum == StatsEnum.ALL || statsEnum == StatsEnum.ERES)
                {
                    for (int i = 0; i < ElementalRES.Count; i++)
                    {
                        switch (upgradeDetail.Method)
                        {
                            case StatUpgradeDetail.MethodEnums.Additive:
                                ElementalRES[i] += upgradeDetail.Amount;
                                break;
                            case StatUpgradeDetail.MethodEnums.Multiplicative:
                                ElementalRES[i] *= upgradeDetail.Amount;
                                break;
                            case StatUpgradeDetail.MethodEnums.Overwrite:
                                ElementalRES[i] = upgradeDetail.Amount;
                                break;
                        }
                    }
                }
            }
        }

        canEmitOnStatChanged = true;
        OnStatsChanged?.Invoke();
    }

    private void OnStatsChangedAdapter(NetworkListEvent<float> networkListEvent)
    {
        if (!canEmitOnStatChanged) return;
        OnStatsChanged?.Invoke();
    }

    [ClientRpc]
    private void ServerSendStatsReadyClientRpc(bool isReady)
    {
        // No need to have isStatsReady as NetworkVariable, it's inefficient 
        isStatsReady = isReady;
    }
}

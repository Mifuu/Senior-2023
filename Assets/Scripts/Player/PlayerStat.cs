using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class PlayerStat : NetworkBehaviour
{
    public enum StatsEnum { ATK, DEF, CritDMG, CritRate, EDMG, ERES, ALL }

    #region Stats Default Value

    [SerializeField] private float defaultATK = 1.0f;
    [SerializeField] private float defaultDEF = 1.0f;
    [SerializeField] private float defaultCritDMG = 1.0f;
    [SerializeField] private float defaultCritRate = 1.0f;
    [SerializeField] private float defaultElementalDMGBonus = 1.0f;
    [SerializeField] private float defaultElementalRES = 1.0f;

    #endregion

    #region Stats Internal

    private NetworkList<float> BaseStat = new NetworkList<float>();
    private NetworkList<float> ElementalDMGBonus = new NetworkList<float>();
    private NetworkList<float> ElementalRES = new NetworkList<float>();

    #endregion

    #region Stats

    public float BaseATK { get => BaseStat[(int)StatsEnum.ATK]; set => BaseStat[(int)StatsEnum.ATK] = value; }
    public float BaseDEF { get => BaseStat[(int)StatsEnum.DEF]; set => BaseStat[(int)StatsEnum.DEF] = value; }
    public float CritDMG { get => BaseStat[(int)StatsEnum.CritDMG]; set => BaseStat[(int)StatsEnum.CritDMG] = value; }
    public float CritRate { get => BaseStat[(int)StatsEnum.CritRate]; set => BaseStat[(int)StatsEnum.CritRate] = value; }

    // public NetworkVariable<float> BaseATK = new NetworkVariable<float>(-1f);
    // public NetworkVariable<float> BaseDEF = new NetworkVariable<float>(-1f);
    // public NetworkVariable<float> CritDMG = new NetworkVariable<float>(-1f);
    // public NetworkVariable<float> CritRate = new NetworkVariable<float>(-1f);

    #endregion

    #region Stats Upgrade

    [Serializable]
    public struct StatUpgrageDetail
    {
        public enum MethodEnums { Additive, Multiplicative, Overwrite }
        [SerializeField] public MethodEnums Method;
        [SerializeField] public List<StatsEnum> Stats;
        [SerializeField] public float Amount;
        [SerializeField] public int LevelLowerBoundInclusive;
        [SerializeField] public int LevelUpperBoundExclusive;
    }

    [SerializeField] private List<StatUpgrageDetail> UpgradeDetail = new List<StatUpgrageDetail>();

    #endregion

    private bool isStatsReady = false;

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

        // Like and Subscribe to Player Level change here
        
        ServerSendStatsReadyClientRpc(true);
        throw new System.NotImplementedException("PlayerStats should subscribe to player level change");
    }

    public override void OnDestroy()
    {
        ElementalDMGBonus?.Dispose();
        ElementalRES?.Dispose();
        BaseStat?.Dispose();
    }

    // Used when Adapting/Changing stats when level changed
    public void OnLevelChanged(int prev, int current)
    {
        if (!IsServer) return;
        UpgradeStatsOnLevelChanged(current);
    }

    public float GetElementDMGBonus(ElementalType element) => ElementalDMGBonus[(int)element];

    public float GetElementRES(ElementalType element) => ElementalRES[(int)element];

    // Level Upgrade does not run often, so iterating through a list should be performant enough
    private void UpgradeStatsOnLevelChanged(float newLevel)
    {
        // Writing O(n2) code like a pro
        foreach (StatUpgrageDetail upgradeDetail in UpgradeDetail)
        {
            if (!(newLevel <= upgradeDetail.LevelLowerBoundInclusive && newLevel > upgradeDetail.LevelUpperBoundExclusive)) continue;
            foreach (StatsEnum statsEnum in upgradeDetail.Stats)
            {
                if ((new List<StatsEnum>() { StatsEnum.ATK, StatsEnum.DEF, StatsEnum.CritDMG, StatsEnum.CritRate }).Contains(statsEnum)
                        || statsEnum == StatsEnum.ALL)
                {
                    switch (upgradeDetail.Method)
                    {
                        case StatUpgrageDetail.MethodEnums.Additive:
                            BaseStat[(int)statsEnum] += upgradeDetail.Amount;
                            break;
                        case StatUpgrageDetail.MethodEnums.Multiplicative:
                            BaseStat[(int)statsEnum] *= upgradeDetail.Amount;
                            break;
                        case StatUpgrageDetail.MethodEnums.Overwrite:
                            BaseStat[(int)statsEnum] = upgradeDetail.Amount;
                            break;
                    }
                }

                // WRITING O(N3) CODE LIKE A PRO
                if (statsEnum == StatsEnum.ALL || statsEnum == StatsEnum.EDMG)
                {
                    for (int i = 0; i < ElementalDMGBonus.Count; i++)
                    {
                        switch (upgradeDetail.Method)
                        {
                            case StatUpgrageDetail.MethodEnums.Additive:
                                ElementalDMGBonus[i] += upgradeDetail.Amount;
                                break;
                            case StatUpgrageDetail.MethodEnums.Multiplicative:
                                ElementalDMGBonus[i] *= upgradeDetail.Amount;
                                break;
                            case StatUpgrageDetail.MethodEnums.Overwrite:
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
                            case StatUpgrageDetail.MethodEnums.Additive:
                                ElementalRES[i] += upgradeDetail.Amount;
                                break;
                            case StatUpgrageDetail.MethodEnums.Multiplicative:
                                ElementalRES[i] *= upgradeDetail.Amount;
                                break;
                            case StatUpgrageDetail.MethodEnums.Overwrite:
                                ElementalRES[i] = upgradeDetail.Amount;
                                break;
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    private void ServerSendStatsReadyClientRpc(bool isReady)
    {
        // No need to have isStatsReady as NetworkVariable, it's inefficient 
        isStatsReady = isReady;
    }
}

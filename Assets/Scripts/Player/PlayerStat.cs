using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerStat : NetworkBehaviour
{
    [SerializeField] private float defaultATK = 1.0f;
    [SerializeField] private float defaultDEF = 1.0f;
    [SerializeField] private float defaultCritDMG = 1.0f;
    [SerializeField] private float defaultCritRate = 1.0f;
    [SerializeField] private float defaultElementalDMGBonus = 1.0f;
    [SerializeField] private float defaultElementalRES = 1.0f;

    public NetworkVariable<float> BaseATK = new NetworkVariable<float>(-1f);
    public NetworkVariable<float> BaseDEF = new NetworkVariable<float>(-1f);
    public NetworkVariable<float> CritDMG = new NetworkVariable<float>(-1f);
    public NetworkVariable<float> CritRate = new NetworkVariable<float>(-1f);

    private NetworkList<float> ElementalDMGBonus = new NetworkList<float>();
    private NetworkList<float> ElementalRES = new NetworkList<float>();

    private bool isStatsReady = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer || isStatsReady) return;

        BaseATK.Value = defaultATK;
        BaseDEF.Value = defaultDEF;
        CritDMG.Value = defaultCritDMG;
        CritRate.Value = defaultCritRate;

        foreach (ElementalType elementalType in Enum.GetValues(typeof(ElementalType)))
        {
            ElementalDMGBonus[(int)elementalType] = defaultElementalDMGBonus;
            ElementalRES[(int)elementalType] = defaultElementalRES;
        }

        ServerSendStatsReadyClientRpc(true);
    }

    public void OnLevelChanged(int prev, int current)
    {
        throw new System.NotImplementedException("On Level Changed function not yet implemented");
    }

    public float GetElementDMGBonus(ElementalType element)
    {
        return ElementalDMGBonus[(int)element];
    }

    public float GetElementRES(ElementalType element)
    {
        return ElementalRES[(int)element];
    }

    [ClientRpc]
    private void ServerSendStatsReadyClientRpc(bool isReady)
    {
        // No need to have isStatsReady as NetworkVariable, it's inefficient 
        isStatsReady = isReady;
    }
}

// public struct StatsUpgradeDetail
// {
//     public float amount;
//     public enum method { additive, multiplicative }
//     public enum stats { atk, def, critrate, critdmg, edmg, eres, all }
// }

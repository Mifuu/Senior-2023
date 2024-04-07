using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class BuffManager : NetworkBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerDash playerDash;

    #region SkillCard Network Variables
    public NetworkVariable<float> AtkBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> DefBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> HpBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> CritBuff_SkillCard { get; set; } = new NetworkVariable<float>(0f);
    public NetworkVariable<float> JumpBuff_SkillCard { get; set; } = new NetworkVariable<float>(0f);
    public NetworkVariable<float> DashBuff_SkillCard { get; set; } = new NetworkVariable<float>(0f);
    public NetworkVariable<float> SkillCooldownBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    #endregion

    #region BuffTotal variables
    public float AtkBuffTotal { get; private set; } = 1f; 
    public float DefBuffTotal { get; private set; } = 1f;
    public float HpBuffTotal { get; private set; } = 1f;
    public float CritBuffTotal { get; private set; } = 1f;
    public float JumpBuffTotal { get; private set; } = 0f;
    public float DashBuffTotal { get; private set; } = 0f;
    public float SkillCooldownBuffTotal { get; private set; } = 1f;
    #endregion

    public event Action OnBuffChanged;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerDash = GetComponent<PlayerDash>();

        // Subscribe to the OnValueChanged event for each NetworkVariable
        AtkBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateAtkBuffTotal();
        DefBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateDefBuffTotal();
        HpBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateHpBuffTotal();
        CritBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateCritBuffTotal();
        JumpBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateJumpBuffTotal();
        DashBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateDashBuffTotal();
        SkillCooldownBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateSkillCooldownBuffTotal();

        // Initial calculation
        RecalculateAllBuffTotals();
    }

    #region Recalculate Functions
    private void RecalculateAllBuffTotals() // Recalculate all BuffTotals
    {
        RecalculateAtkBuffTotal();
        RecalculateDefBuffTotal();
        RecalculateHpBuffTotal();
        RecalculateCritBuffTotal();
        RecalculateJumpBuffTotal();
        RecalculateDashBuffTotal();
        RecalculateSkillCooldownBuffTotal();
    }

    private void RecalculateAtkBuffTotal()
    {
        if (!IsOwner) return;
        AtkBuffTotal = AtkBuff_SkillCard.Value;
        OnBuffChanged?.Invoke();
    }

    private void RecalculateDefBuffTotal()
    {
        if (!IsOwner) return;
        DefBuffTotal = DefBuff_SkillCard.Value;
        OnBuffChanged?.Invoke();
    }

    private void RecalculateHpBuffTotal()
    {
        if (!IsOwner) return;
        HpBuffTotal = HpBuff_SkillCard.Value;
        playerHealth.HealthBuffMultiplier.Value = HpBuffTotal;
        OnBuffChanged?.Invoke();
    }

    private void RecalculateCritBuffTotal()
    {
        if (!IsOwner) return;
        CritBuffTotal = CritBuff_SkillCard.Value;
        OnBuffChanged?.Invoke();
    }

    private void RecalculateJumpBuffTotal()
    {
        if (!IsOwner) return;
        JumpBuffTotal = JumpBuff_SkillCard.Value;
        OnBuffChanged?.Invoke();
    }

    // Recalculate Total dash buff and change the buff variable of playerDash script
    private void RecalculateDashBuffTotal()
    {
        if (!IsOwner) return;
        DashBuffTotal = DashBuff_SkillCard.Value;
        playerDash.DashBuffAddition.Value = ((int)DashBuffTotal);
        OnBuffChanged?.Invoke();
    }

    private void RecalculateSkillCooldownBuffTotal()
    {
        if (!IsOwner) return;
        SkillCooldownBuffTotal = SkillCooldownBuff_SkillCard.Value;
        OnBuffChanged?.Invoke();
    }
    #endregion

    
}

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class BuffManager : NetworkBehaviour
{
    private PlayerHealth playerHealth;
    public NetworkVariable<float> AtkBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> DefBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> HpBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> CritBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> JumpBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> DashBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);
    public NetworkVariable<float> SkillCooldownBuff_SkillCard { get; set; } = new NetworkVariable<float>(1f);

    public float AtkBuffTotal = 1;
    public float DefBuffTotal = 1;
    public float HpBuffTotal = 1;
    public float CritBuffTotal = 0;
    public float JumpBuffTotal = 0;
    public float DashBuffTotal = 0;
    public float SkillCooldownBuffTotal = 1;

    // Start is called before the first frame update
    void Start()
    {
       playerHealth = FindObjectOfType<PlayerHealth>();
       HpBuff_SkillCard.OnValueChanged += (prev, current) => RecalculateHpBuffTotal();
        
    }

    private void RecalculateHpBuffTotal()
    {
        if (!IsOwner) return;     
        HpBuffTotal = HpBuff_SkillCard.Value;
        playerHealth.HealthBuffMultiplier.Value = HpBuffTotal;
    }

}

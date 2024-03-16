using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCard_HpBuff : SkillCard_StatModifier
{
    private PlayerHealth playerHealth;
    protected override void ApplyModifier()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerHealth.HealthBuffMultiplier.Value += (skillCard.Multiplier - 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCard_HpBuff : SkillCard
{
    protected override void ApplyModifier()
    {
        buffManager.HpBuff_SkillCard.Value = skillCard.Multiplier;
    }
}

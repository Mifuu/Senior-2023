using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCard_AtkBuff : SkillCard
{
    protected override void ApplyModifier()
    {
        buffManager.AtkBuff_SkillCard.Value = skillCard.Multiplier;
    }
}

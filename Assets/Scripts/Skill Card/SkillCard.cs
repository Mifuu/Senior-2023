using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillCard : NetworkBehaviour
{
    public SkillCardScriptableObject skillCard; // assign the SkillCardScriptableObject corresponding to the carpType in unity inspector
    [HideInInspector] public BuffManager buffManager;// find buffManager component in player prefab 

    public enum CardType
    {
       Atk,
       Def,
       Hp,
       Crit,
       Jump,
       Dash,
       SkillCooldown,
       HpRegen,
    }

    public CardType cardType;

    private void Start()
    {
        buffManager = FindObjectOfType<BuffManager>();
        switch (cardType)
        {
            case CardType.Atk:
                buffManager.AtkBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.Def:
                buffManager.DefBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.Hp:
                buffManager.HpBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.Crit:
                buffManager.CritBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.Jump:
                buffManager.JumpBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.Dash:
                buffManager.DashBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.SkillCooldown:
                buffManager.SkillCooldownBuff_SkillCard.Value = skillCard.Multiplier;
                break;
            case CardType.HpRegen: // not yet implemented
                break;
            default:
                Debug.LogError($"Unhandled card type: {cardType}");
                break;
        }
        ApplyModifier(); 
    }

    protected virtual void ApplyModifier()
    {

    }
}
   

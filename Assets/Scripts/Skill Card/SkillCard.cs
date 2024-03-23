using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillCard : NetworkBehaviour
{
    public SkillCardScriptableObject skillCardData; // Assign the SkillCardScriptableObject corresponding to the carpType in unity inspector
    [HideInInspector] public BuffManager buffManager; // Find buffManager component in player prefab 

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
        buffManager = PlayerManager.Instance.gameObject.GetComponent<BuffManager>();
        switch (cardType)
        {
            case CardType.Atk:
                buffManager.AtkBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.Def:
                buffManager.DefBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.Hp:
                buffManager.HpBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.Crit:
                buffManager.CritBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.Jump: // Not yet Implemented
                buffManager.JumpBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.Dash:
                buffManager.DashBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.SkillCooldown:
                buffManager.SkillCooldownBuff_SkillCard.Value = skillCardData.Multiplier;
                break;
            case CardType.HpRegen: // Not yet implemented
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
   

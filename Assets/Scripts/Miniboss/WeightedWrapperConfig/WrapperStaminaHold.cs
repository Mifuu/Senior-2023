using System;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "StaminaHold", menuName = "Miniboss/Weighted Attack Wrapper/stamina Hold")]
    public class WrapperStaminaHold : WeightedEnemyAttack
    {
        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            if (staminaManager.hasEnoughStamina(requiredStamina)) 
                return EnemyWeightedAttackResponseMode.Proceed;
            else 
                return EnemyWeightedAttackResponseMode.Hold;
        }

        public override void GenerateHoldingCallback(Action callback)
        {
            staminaManager.GenerateStaminaLevelCallback(requiredStamina, callback);
        }
    }
}

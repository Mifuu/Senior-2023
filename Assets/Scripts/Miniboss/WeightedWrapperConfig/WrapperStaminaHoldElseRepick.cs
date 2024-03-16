using System;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "StaminaHoldElseRepick", menuName = "Miniboss/Weighted Attack Wrapper/Stamina Hold Else Repick")]
    public class WrapperStaminaHoldElseRepick : WeightedEnemyAttack
    {
        [SerializeField] private int staminaDiffThreshold;

        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            if (staminaManager.hasEnoughStamina(requiredStamina))
                return EnemyWeightedAttackResponseMode.Proceed;
            else
            {
                if (staminaManager.compareStamina(requiredStamina) <= (-1 * staminaDiffThreshold))
                {
                    return EnemyWeightedAttackResponseMode.Repick;
                }
                return EnemyWeightedAttackResponseMode.Hold;
            }
        }

        public override void GenerateHoldingCallback(Action callback)
        {
            staminaManager.GenerateStaminaLevelCallback(requiredStamina, callback);
        }
    }
}

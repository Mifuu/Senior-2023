using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "StaminaRepick", menuName = "Miniboss/Weighted Attack Wrapper/Stamina Repick")]
    public class WrapperStaminaRepick : WeightedEnemyAttack
    {
        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            if (staminaManager.hasEnoughStamina(requiredStamina))
            {
                return EnemyWeightedAttackResponseMode.Proceed;
            }

            return EnemyWeightedAttackResponseMode.Repick;
        }
    }
}

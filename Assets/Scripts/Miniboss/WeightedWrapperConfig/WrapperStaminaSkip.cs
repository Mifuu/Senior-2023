using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "StaminaSkip", menuName = "Miniboss/Weighted Attack Wrapper/Stamina Skip")]
    public class WrapperStaminaSkip : WeightedEnemyAttack
    {
        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            if (staminaManager.hasEnoughStamina(requiredStamina))
            {
                return EnemyWeightedAttackResponseMode.Proceed;
            }

            return EnemyWeightedAttackResponseMode.Skip;
        }
    }
}

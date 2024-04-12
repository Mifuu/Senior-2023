using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EmptyAttack", menuName = "Enemy/Enemy Logic/Attack Pattern/Empty")]
    public class EmptyAttack : EnemyAttack
    {
        public override void PerformAttack() 
        {
            base.PerformAttack();
            EmitAttackEndsEvent();
        }
    }
}

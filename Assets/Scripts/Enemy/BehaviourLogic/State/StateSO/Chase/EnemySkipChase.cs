using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "SkipChase", menuName = "Enemy/Enemy State/Chase State/Skip")]
    public class EnemySkipChase : EnemyChaseSOBase
    {
        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }
    }
}

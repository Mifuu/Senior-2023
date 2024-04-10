using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "Cooldown", menuName = "Enemy/Enemy State/Knockback State/Cooldown")]
    public class EnemyKnockbackCooldown : EnemyKnockbackSOBase
    {
        [SerializeField] float cooldownTime = 5.0f;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            enemy.StartCoroutine(Cooldown());
        }

        public override void DoExitLogic()
        {
            base.DoExitLogic();
            enemy.StopCoroutine(Cooldown());
        }

        public IEnumerator Cooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TimedOrchestrated", menuName = "Miniboss/Orchestration Attack/Timed")]
    public class TimedOrchestratedAttack : OrchestrationAttack
    {
        [SerializeField] private float timeBetweenAttacks;

        public override void PerformAttack()
        {
            enemy.StartCoroutine(AttackSequence());
        }

        private IEnumerator AttackSequence()
        {
            foreach (var attack in listOfOrchestratedAttacks)
            {
                attack.PerformAttack();
                yield return new WaitForSeconds(timeBetweenAttacks);
            }

            EmitAttackEndsEvent();
        }
    }
}

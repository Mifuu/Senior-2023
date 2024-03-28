using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Enemy
{
    [CreateAssetMenu(fileName = "RandomNOrchestrated", menuName = "Miniboss/Orchestration Attack/Random N")]
    public class RandomNOrchestratedAttack : OrchestrationAttack
    {
        private int currentAttackAmount = 0;
        private System.Random rnd = new System.Random();
        [SerializeField] private bool withSelf;

        public override void PerformAttack()
        {
            if (withSelf)
            {
                instantiatedAttack.PerformAttack();
            }

            foreach (var attack in GetAttacksRandomNumber())
            {
                attack.PerformAttack();
            }

            currentAttackAmount += 1;
        }

        public List<EnemyAttack> GetAttacksRandomNumber()
        {
            return GetAttacks().OrderBy((x) => rnd.Next()).Take(currentAttackAmount).ToList();
        }
    }
}

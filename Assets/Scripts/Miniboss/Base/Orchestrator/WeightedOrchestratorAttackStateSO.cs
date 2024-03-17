using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy State/Attack State/Orchestrated Weighted Attack", fileName = "OrchestratedWeightedAttackState")]
    public class WeightedOrchestratorAttackStateSO : WeightAttackPoolAttackStateSO
    {
        [Header("Default Hijacked Attack State")]
        [SerializeField] private EnemyAttackSOBase attackState;
        private List<OrchestrationAttack> listOfOrchestrationAttack = new List<OrchestrationAttack>();
        private OrchestratedSpawnManager spawnManager; 
        private EnemyStateMachine stateMachine;

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            int oCount = 0; // Debug Purpose Only
            foreach (var attack in weightedAttacks)
            {
                if (attack.attack is OrchestrationAttack)
                {
                    listOfOrchestrationAttack.Add(attack.attack as OrchestrationAttack);
                    oCount++;
                }
            }

            spawnManager = gameObject.GetComponent<OrchestratedSpawnManager>();
            stateMachine = enemy.GetComponent<EnemyStateMachine>();

            Debug.Log($"There are {oCount} Orchestrated Attack");

            if (spawnManager != null)
            {
                spawnManager.OnEnemySpawns += HijackAttackState;
            }
        }

        private void HijackAttackState(List<EnemyBase> enemies)
        {
            foreach (var enemy in enemies)
            {
                EnemyStateSynchronizable synchronizable;
                if (enemy.TryGetComponent<EnemyStateSynchronizable>(out synchronizable))
                {
                    synchronizable.RequestSynchronize(stateMachine); 
                }
                
                EnemyStateHijackable hijackable;
                if (enemy.TryGetComponent<EnemyStateHijackable>(out hijackable))
                {
                    var returnedState = hijackable.HijackAttackStateInitializeOnly(FormAttackState());
                    for (int i = 0; i < listOfOrchestrationAttack.Count; i++)
                    {
                        listOfOrchestrationAttack[i].listOfOrchestratedAttacks.Add(returnedState.allAttack[i]);
                    }
                }
            }
        }

        // TODO: Teardown State and Attack Sync on enemy die
        
        private EnemyAttackSOBase FormAttackState()
        {
            var instantiatedAttackState = Instantiate(attackState);
            var listOfInstantiatedAttacks = new List<EnemyAttack>();
            
            foreach (var atk in listOfOrchestrationAttack)
            {
                var insAttack = Instantiate(atk.attack);
                listOfInstantiatedAttacks.Add(insAttack);
            }

            instantiatedAttackState.allAttack.AddRange(listOfInstantiatedAttacks);
            return instantiatedAttackState;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy/Enemy State/Attack State/Orchestrated Weighted Attack", fileName = "OrchestratedWeightedAttackState")]
    public class WeightedOrchestratorAttackStateSO : WeightAttackPoolAttackStateSO
    {
        [Header("Default Hijacked Attack State")]
        [SerializeField] private EnemyAttackSOBase attackState;
        [SerializeField] private string spawnManagerId;
        private List<OrchestrationAttack> listOfOrchestrationAttack = new List<OrchestrationAttack>();
        private OrchestratedSpawnManager spawnManager;
        private EnemyStateMachine stateMachine;
        private Dictionary<ulong, EnemyAttackSOBase> aliveEnemyState = new Dictionary<ulong, EnemyAttackSOBase>();

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

            stateMachine = enemy.GetComponent<EnemyStateMachine>();
            var allSpawnManager = gameObject.GetComponentsInChildren<OrchestratedSpawnManager>();
            foreach (var manager in allSpawnManager)
            {
                Debug.Log(manager.UniqueId);
                if (manager.UniqueId == spawnManagerId)
                    spawnManager = manager;
            }

            // Debug.Log($"There are {oCount} Orchestrated Attack");

            if (spawnManager != null)
            {
                spawnManager.OnEnemySpawns += HijackAttackState;
                spawnManager.OnEnemyDies += OnEnemyDiesTeardown;
                return;
            }

            Debug.LogError("SpawnManager can not be found");
        }

        public void OnDestroy()
        {
            spawnManager.OnEnemyDies -= OnEnemyDiesTeardown;
        }

        // Used with OnEnemySpawn from the Spawner
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
                    try
                    {
                        var enemyId = enemy.GetComponent<NetworkObject>().NetworkObjectId;
                        var returnedState = hijackable.HijackAttackStateInitializeOnly(FormAttackState());
                        aliveEnemyState.Add(enemyId, returnedState);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        private void OnEnemyDiesTeardown(EnemyBase enemy)
        {
            if (!aliveEnemyState.Remove(enemy.GetComponent<NetworkObject>().NetworkObjectId))
                Debug.LogError("Can not find enemy by its network id");
        }

        private EnemyAttackSOBase FormAttackState()
        {
            var instantiatedAttackState = Instantiate(attackState);
            var listOfInstantiatedAttacks = new List<EnemyAttack>();

            for (int i = 0; i < listOfOrchestrationAttack.Count; i++)
            {
                var atk = listOfOrchestrationAttack[i];
                var insOrchestratedAtk = Instantiate(atk);

                var insAttack = Instantiate(insOrchestratedAtk.attack);
                listOfInstantiatedAttacks.Add(insAttack);

                atk.AssignOrchestrationController(this, i);
            }

            instantiatedAttackState.allAttack = new List<EnemyAttack>();
            instantiatedAttackState.allAttack.AddRange(listOfInstantiatedAttacks);
            return instantiatedAttackState;
        }

        public List<EnemyAttack> GetAllAttacksFromIndex(int index) => aliveEnemyState.Values.Select((state) => state.allAttack[index]).ToList();
    }
}

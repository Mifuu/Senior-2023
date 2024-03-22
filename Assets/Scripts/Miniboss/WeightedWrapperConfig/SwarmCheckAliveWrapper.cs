using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    // Note: This is just a more generalized version of the Normal SwarmWrapper
    [CreateAssetMenu(fileName = "SwarmCheckAliveWrapper", menuName = "Miniboss/Weighted Attack Wrapper/Swarm Check Alive")]
    public class SwarmCheckAliveWrapper : WrapperStaminaRepick
    {
        [Header("Spawn Manager Setting")]
        [SerializeField] private string spawnManagerId;

        [Header("Ready Condition")]
        [SerializeField] private bool strictEqual;
        [SerializeField] private bool isMorethan;
        [SerializeField] private bool equalTo;
        [SerializeField] private int targetValue;

        private PersonalEnemySpawnManager spawnManager;
        private bool IsReady = true;

        private void UnreadyAttack(List<EnemyBase> enemyList) => IsReady = false;
        private void ReadyAttack() => IsReady = true;

        public override void Initialize(GameObject targetPlayer, GameObject enemy)
        {
            base.Initialize(targetPlayer, enemy);
            var allSpawnManager = enemy.GetComponentsInChildren<PersonalEnemySpawnManager>();
            foreach (var manager in allSpawnManager)
            {
                if (manager.UniqueId == spawnManagerId)
                    this.spawnManager = manager;
            }

            if (spawnManager == null)
                Debug.LogError("Personal Spawn Manager not found");

            spawnManager.currentAliveEnemy.OnValueChanged += CheckAndChangeAttackReadiness;
        }

        private void CheckAndChangeAttackReadiness(int prev, int current)
        {
            if (strictEqual)
                IsReady = (current == targetValue);
            else
            {
                if (isMorethan)
                    IsReady = (current > targetValue && (!equalTo || current == targetValue));
                else
                    IsReady = (current < targetValue && (!equalTo || current == targetValue));
            }
        }

        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            var staminaCheck = base.CheckAndActivateAttack();
            if (staminaCheck == EnemyWeightedAttackResponseMode.Proceed)
            {
                if (IsReady) 
                    return EnemyWeightedAttackResponseMode.Proceed;
                return EnemyWeightedAttackResponseMode.Repick;
            }
            return staminaCheck;
        }

        public void OnDestroy()
        {
            spawnManager.currentAliveEnemy.OnValueChanged -= CheckAndChangeAttackReadiness;
        }
    }
}

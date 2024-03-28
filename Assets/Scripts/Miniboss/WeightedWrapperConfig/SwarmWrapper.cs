using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    [CreateAssetMenu(fileName = "SwarmHold", menuName = "Miniboss/Weighted Attack Wrapper/Swarm")]
    public class SwarmWrapper : WrapperStaminaHold
    {
        [Header("Spawn Manager Setting")]
        [SerializeField] private string spawnManagerId;

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

            spawnManager.OnEnemySpawns += UnreadyAttack;
            spawnManager.OnAllEnemyDies += ReadyAttack;
        }

        public void OnDestroy()
        {
            spawnManager.OnEnemySpawns -= UnreadyAttack;
            spawnManager.OnAllEnemyDies -= ReadyAttack;
        }

        public override EnemyWeightedAttackResponseMode CheckAndActivateAttack()
        {
            var staminaCheck = base.CheckAndActivateAttack();
            if (staminaCheck == EnemyWeightedAttackResponseMode.Proceed)
            {
                if (IsReady)
                    return EnemyWeightedAttackResponseMode.Proceed;
                return EnemyWeightedAttackResponseMode.Hold;
            }
            return staminaCheck;
        }
    }
}

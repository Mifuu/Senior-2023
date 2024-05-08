using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public class LustBase : EnemyBase
    {
        [Header("Lust Miniboss Config")]
        [SerializeField] private MinibossFightRangeController rangeController;
        [SerializeField] private OrchestratedSpawnManager statueSpawnManager;

        public override void Awake()
        {
            base.Awake();
            statueSpawnManager.OnEnemySpawns += SetupStatue;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            statueSpawnManager.OnEnemySpawns -= SetupStatue;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            rangeController.OnMinibossFightExit += ResetBossFight;
            rangeController.OnMinibossFightEnter += StartBossFight;
            GameplayUIBossHP.instance.OpenHealthBar(currentHealth, maxHealth, "Lust");
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            rangeController.OnMinibossFightExit -= ResetBossFight;
            rangeController.OnMinibossFightEnter -= StartBossFight;
            GameplayUIBossHP.instance.CloseHealthBar();
        }

        private void StartBossFight()
        {
            SetupBehaviour();
            StateMachine.StartStateMachine();
        }

        private void ResetBossFight()
        {
            currentHealth.Value = networkMaxHealth.Value;
            StateMachine.StopStateMachine();
            StateMachine.ResetStateMachine();
            statueSpawnManager.KillAllSpawnedEnemy(null);
            statueSpawnManager.Spawn();
        }

        private void SetupStatue(List<EnemyBase> enemyBases)
        {
            for (int i = 0; i < enemyBases.Count; i++)
                enemyBases[i].SetupBehaviour();
        }
    }
}

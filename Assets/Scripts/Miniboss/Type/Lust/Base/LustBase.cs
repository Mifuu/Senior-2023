using UnityEngine;

namespace Enemy
{
    public class LustBase : EnemyBase
    {
        [Header("Lust Miniboss Config")]
        [SerializeField] private MinibossFightRangeController rangeController;
        [SerializeField] private OrchestratedSpawnManager statueSpawnManager;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            rangeController.OnMinibossFightExit += ResetBossFight;
            rangeController.OnMinibossFightEnter += StateMachine.StartStateMachine;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            rangeController.OnMinibossFightExit -= ResetBossFight;
            rangeController.OnMinibossFightEnter -= StateMachine.StartStateMachine;
        }

        private void ResetBossFight()
        {
            currentHealth.Value = networkMaxHealth.Value;
            StateMachine.StopStateMachine();
            StateMachine.ResetStateMachine();
            statueSpawnManager.KillAllSpawnedEnemy();
            statueSpawnManager.Spawn();
        }
    }
}

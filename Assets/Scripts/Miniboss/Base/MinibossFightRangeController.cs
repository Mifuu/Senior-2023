using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public class MinibossFightRangeController : NetworkBehaviour
    {
        [Header("Self Spawn Option")]
        [SerializeField] private EnemyWithinTriggerCheck enterFightTriggerCheck;
        [SerializeField] private EnemyWithinTriggerCheck exitFightTriggerCheck;

        [Tooltip("When true, will spawn its own player enter room check using TriggerCheck Object provided in enterFightTriggerCheck")]
        [SerializeField] private bool spawnEnterCheckImmediate;
        [Tooltip("When true, will spawn its own player exit room check using TriggerCheck Object provided in exitFightTriggerCheck")]
        [SerializeField] private bool spawnExitCheckImmediate;

        [Header("State Machine Set up")]
        [SerializeField] private EnemyStateMachine stateMachine;

        private EnemyWithinTriggerCheck enterFightCheckInstance;
        private EnemyWithinTriggerCheck exitFightCheckInstance;
        private NetworkVariable<bool> isFightActivated = new NetworkVariable<bool>(false);

        public event Action OnMinibossFightEnter;
        public event Action OnMinibossFightExit; // Does not fire when the miniboss die

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (spawnEnterCheckImmediate && enterFightTriggerCheck != null)
            {
                var checkInstance = Instantiate(enterFightTriggerCheck, transform.position, Quaternion.identity);
                if (checkInstance.TryGetComponent<NetworkObject>(out var networkObject))
                    networkObject.Spawn();
                SetupEnterTrigger(checkInstance);
            }

            if (spawnExitCheckImmediate && exitFightTriggerCheck != null)
            {
                var checkInstance = Instantiate(exitFightTriggerCheck, transform.position, Quaternion.identity);
                if (checkInstance.TryGetComponent<NetworkObject>(out var networkObject))
                    networkObject.Spawn();
                SetupExitTrigger(checkInstance);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            DesetupExitTrigger();
            DesetupEnterTrigger();
        }

        public void SetEnterTrigger(EnemyWithinTriggerCheck check)
        {
            DesetupEnterTrigger();
            SetupEnterTrigger(check);
        }

        public void SetExitTrigger(EnemyWithinTriggerCheck check)
        {
            DesetupExitTrigger();
            SetupExitTrigger(check);
        }

        private void SetupEnterTrigger(EnemyWithinTriggerCheck check)
        {
            if (enterFightCheckInstance != null)
                DesetupEnterTrigger();

            if (check == null) return;
            enterFightCheckInstance = check;
            enterFightCheckInstance.OnPlayerEnteringTrigger += CheckAndActivateFight;
        }

        private void SetupExitTrigger(EnemyWithinTriggerCheck check)
        {
            if (exitFightCheckInstance != null)
                DesetupExitTrigger();

            if (check == null) return;
            exitFightCheckInstance = check;
            exitFightCheckInstance.OnPlayerLeavingTrigger += CheckAndDeactivateFight;
        }

        private void DesetupEnterTrigger()
        {
            if (enterFightCheckInstance == null) return;
            enterFightCheckInstance.OnPlayerEnteringTrigger -= CheckAndActivateFight;
            enterFightCheckInstance = null;
        }

        private void DesetupExitTrigger()
        {
            if (exitFightCheckInstance == null) return;
            exitFightCheckInstance.OnPlayerLeavingTrigger -= CheckAndDeactivateFight;
            exitFightCheckInstance = null;
        }

        private void CheckAndActivateFight(GameObject player, int playerInTriggerCount)
        {
            if (playerInTriggerCount > 0 && !isFightActivated.Value && IsServer)
            {
                isFightActivated.Value = true;
                stateMachine.StartStateMachine();
                OnMinibossFightEnter?.Invoke();
            }
        }

        private void CheckAndDeactivateFight(GameObject player, int playerInTriggerCount)
        {
            if (playerInTriggerCount == 0 && isFightActivated.Value && IsServer)
            {
                isFightActivated.Value = false;
                stateMachine.StopStateMachine();
                stateMachine.ResetStateMachine();
                OnMinibossFightExit?.Invoke();
            }
        }

    }
}

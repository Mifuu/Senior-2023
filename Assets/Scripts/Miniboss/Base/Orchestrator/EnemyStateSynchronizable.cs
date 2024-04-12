using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    [RequireComponent(typeof(EnemyStateMachine))]
    public class EnemyStateSynchronizable : NetworkBehaviour
    {
        private EnemyStateMachine stateMachine;
        private NetworkVariable<bool> isSync = new NetworkVariable<bool>(false);
        private EnemyStateMachine currentParentStateMachine;

        public void Awake()
        {
            stateMachine = GetComponent<EnemyStateMachine>(); 
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!isSync.Value) return;
            StopSynchronize(currentParentStateMachine);
        }
            
        public void RequestSynchronize(EnemyStateMachine stateMachine)
        {
            if (!IsServer) return;
            if (isSync.Value)
            {
                Debug.LogError("State is already in sync");
                return;
            }

            this.stateMachine.networkEnemyState.OnValueChanged -= this.stateMachine.SynchronizeState;
            stateMachine.networkEnemyState.OnValueChanged += this.stateMachine.SynchronizeState;
            stateMachine.stateMachineState.OnValueChanged += SyncStateMachineState;
            currentParentStateMachine = stateMachine;
            isSync.Value = true;
        }

        private void SyncStateMachineState(EnemyStateMachine.AvailableStateMachineState prev, EnemyStateMachine.AvailableStateMachineState current)
        {
            if (prev == EnemyStateMachine.AvailableStateMachineState.NotStarted && current == EnemyStateMachine.AvailableStateMachineState.Running)
                this.stateMachine.StartStateMachine();
            else if (prev == EnemyStateMachine.AvailableStateMachineState.Running && current == EnemyStateMachine.AvailableStateMachineState.Paused)
                this.stateMachine.ChangePauseStateMachine(true);
            else if (prev == EnemyStateMachine.AvailableStateMachineState.Paused && current == EnemyStateMachine.AvailableStateMachineState.Running)
                this.stateMachine.ChangePauseStateMachine(false);
            else if (prev == EnemyStateMachine.AvailableStateMachineState.Running && current == EnemyStateMachine.AvailableStateMachineState.Stopped)
                this.stateMachine.StopStateMachine();
            else if (prev == EnemyStateMachine.AvailableStateMachineState.Stopped && current == EnemyStateMachine.AvailableStateMachineState.Running)
                this.stateMachine.ResetStateMachine();
        }

        public void StopSynchronize(EnemyStateMachine stateMachine)
        {
            if (!IsServer) return;
            if (!isSync.Value)
            {
                Debug.LogError("State is already is not in sync");
                return;
            }

            this.stateMachine.networkEnemyState.OnValueChanged += this.stateMachine.SynchronizeState;
            stateMachine.networkEnemyState.OnValueChanged -= this.stateMachine.SynchronizeState;
            stateMachine.stateMachineState.OnValueChanged -= SyncStateMachineState;
            currentParentStateMachine = null;
            isSync.Value = false;
        }
    }
}

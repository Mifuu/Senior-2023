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
            currentParentStateMachine = stateMachine;
            isSync.Value = true;
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
            currentParentStateMachine = null;
            isSync.Value = false;
        }
    }
}

using Unity.Netcode;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : NetworkBehaviour
    {
        public EnemyState CurrentEnemyState { get; set; }
        private NetworkVariable<NetworkString> stateSynchronizer = new NetworkVariable<NetworkString>("Idle");
        private EnemyBase enemy;
        private EnemyState InitialState { get; set; }

        public void Start()
        {
            enemy = GetComponent<EnemyBase>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            stateSynchronizer.Value = "Idle";
            stateSynchronizer.OnValueChanged += SynchronizeState;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            CurrentEnemyState = InitialState; 
            stateSynchronizer.OnValueChanged -= SynchronizeState;
        }

        public void Initialize(EnemyState startingState)
        {
            CurrentEnemyState = startingState;
            InitialState = startingState;
            CurrentEnemyState.EnterState();
        }

        public void ChangeState(EnemyState newState)
        {
            if (!IsServer) return;
            stateSynchronizer.Value = newState.stateId;
        }

        private void SynchronizeState(NetworkString _, NetworkString current)
        {
            EnemyState newState;

            switch (current)
            {
                case "Idle":
                    newState = enemy.IdleState;
                    break;
                case "Chase":
                    newState = enemy.ChaseState;
                    break;
                case "Attack":
                    newState = enemy.AttackState;
                    break;
                case "Knockback":
                    newState = enemy.KnockbackState;
                    break;
                default:
                    Debug.LogError("State Synchronization ID Error: " + current);
                    return;
            }

            CurrentEnemyState.ExitState();
            CurrentEnemyState = newState;
            CurrentEnemyState.EnterState();
        }
    }
}

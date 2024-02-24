using Unity.Netcode;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : NetworkBehaviour
    {
        public enum AvailableEnemyState { None, Idle, Chase, Attack, Knockback }
        public EnemyState CurrentEnemyState { get; set; }
        public NetworkVariable<AvailableEnemyState> networkEnemyState = new NetworkVariable<AvailableEnemyState>(AvailableEnemyState.None);

        private EnemyBase enemy;
        private EnemyState startingState;
        private bool isInitialized;

        public void Start()
        {
            enemy = GetComponent<EnemyBase>();
        }

        public void Initialize(EnemyState startingState)
        {
            this.startingState = startingState;
            this.isInitialized = true;
            StartStateMachine();
        }

        public void StartStateMachine()
        {
            if (!IsServer) return;

            networkEnemyState.Value = startingState.stateId;
            CurrentEnemyState = startingState;
            CurrentEnemyState.EnterState();
        }

        public void ChangeState(EnemyState newState)
        {
            if (!IsServer) return;
            networkEnemyState.Value = newState.stateId;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            
            networkEnemyState.OnValueChanged += SynchronizeState;
            if (isInitialized) StartStateMachine();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!IsServer) return;

            networkEnemyState.Value = AvailableEnemyState.None;
            networkEnemyState.OnValueChanged -= SynchronizeState;
        }

        private void SynchronizeState(AvailableEnemyState _, AvailableEnemyState current)
        {
            EnemyState newState;
            switch (current)
            {
                case AvailableEnemyState.Idle:
                    newState = enemy.IdleState;
                    break;
                case AvailableEnemyState.Chase:
                    newState = enemy.ChaseState;
                    break;
                case AvailableEnemyState.Attack:
                    newState = enemy.AttackState;
                    break;
                case AvailableEnemyState.Knockback:
                    newState = enemy.KnockbackState;
                    break;
                case AvailableEnemyState.None:
                    return;
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

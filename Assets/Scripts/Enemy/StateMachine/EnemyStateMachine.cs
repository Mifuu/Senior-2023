using Unity.Netcode;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(EnemyStateMachine))]
    public class EnemyStateMachine : NetworkBehaviour
    {
        public EnemyState CurrentEnemyState { get; set; }
        public NetworkVariable<NetworkString> networkEnemyState = new NetworkVariable<NetworkString>("Idle");
        private EnemyBase enemy;

        public void Start()
        {
            enemy = GetComponent<EnemyBase>();
        }

        public void Initialize(EnemyState startingState)
        {
            CurrentEnemyState = startingState;
            CurrentEnemyState.EnterState();
            networkEnemyState.OnValueChanged += SynchronizeState;
        }

        public void ChangeState(EnemyState newState)
        {
            if (!IsServer) return;
            networkEnemyState.Value = newState.stateId;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            networkEnemyState.OnValueChanged -= SynchronizeState;
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

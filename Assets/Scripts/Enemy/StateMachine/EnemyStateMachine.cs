using Unity.Netcode;
using UnityEngine;

namespace Enemy
{
    public class EnemyStateMachine : NetworkBehaviour
    {
        public enum AvailableEnemyState { None, Idle, Chase, Attack, Knockback, Return }
        public enum AvailableStateMachineState { NotStarted, Running, Paused, Stopped }

        public EnemyState CurrentEnemyState { get; set; }
        public NetworkVariable<AvailableEnemyState> networkEnemyState = new NetworkVariable<AvailableEnemyState>(AvailableEnemyState.None);
        public NetworkVariable<AvailableStateMachineState> stateMachineState
            = new NetworkVariable<AvailableStateMachineState>(AvailableStateMachineState.NotStarted);

        [SerializeField] private bool startStateMachineOnInitialize = true;

        private EnemyBase enemy;
        private EnemyState startingState;
        private bool isInitialized = false;
        private AvailableEnemyState temporaryStateHolder = AvailableEnemyState.Idle;

        public void Awake()
        {
            enemy = GetComponent<EnemyBase>();
        }

        public void Initialize(EnemyState startingState)
        {
            this.startingState = startingState;
            this.isInitialized = true;
            if (startStateMachineOnInitialize) StartStateMachine();
        }

        public void StartStateMachine()
        {
            Debug.Log("Starting State Machine");
            if (stateMachineState.Value != AvailableStateMachineState.NotStarted)
            {
                Debug.LogWarning("State Machine is already started");
                return;
            }

            if (!enemy.enabled) enemy.enabled = true;

            stateMachineState.Value = AvailableStateMachineState.Running;
            // networkEnemyState.Value = startingState.stateId;
            CurrentEnemyState = startingState;

            if (!IsServer) return;
            CurrentEnemyState.EnterState();
        }

        public void ChangePauseStateMachine(bool isPause)
        {
            if (!IsServer) return;

            if (stateMachineState.Value == AvailableStateMachineState.Running && isPause)
            {
                temporaryStateHolder = networkEnemyState.Value;
                stateMachineState.Value = AvailableStateMachineState.Paused;
                networkEnemyState.Value = AvailableEnemyState.Idle;
            }

            if (stateMachineState.Value == AvailableStateMachineState.Paused && !isPause)
            {
                stateMachineState.Value = AvailableStateMachineState.Running;
                networkEnemyState.Value = temporaryStateHolder;
                temporaryStateHolder = AvailableEnemyState.Idle;
            }
        }

        public void StopStateMachine()
        {
            if (!IsServer) return;

            /* if (stateMachineState.Value != AvailableStateMachineState.Running) */
            /* { */
            /*     Debug.LogWarning("Can not stop state machine at current state"); */
            /*     return; */
            /* } */

            CurrentEnemyState.ExitState();
            stateMachineState.Value = AvailableStateMachineState.Stopped;
            networkEnemyState.Value = AvailableEnemyState.Idle;
        }

        public void ResetStateMachine()
        {
            if (!IsServer) return;

            if (stateMachineState.Value != AvailableStateMachineState.Stopped)
            {
                Debug.LogWarning("Can not reset state machine at current state");
                return;
            }

            stateMachineState.Value = AvailableStateMachineState.NotStarted;
            networkEnemyState.Value = startingState.stateId;
        }

        public void ChangeState(EnemyState newState)
        {
            if (!IsServer || stateMachineState.Value != AvailableStateMachineState.Running) return;
            networkEnemyState.Value = newState.stateId;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;

            networkEnemyState.OnValueChanged += SynchronizeState;
            if (isInitialized && startStateMachineOnInitialize)
                StartStateMachine();
            else
                enemy.enabled = false;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            networkEnemyState.OnValueChanged -= SynchronizeState;

            if (!IsServer) return;
            networkEnemyState.Value = startingState.stateId;
            stateMachineState.Value = AvailableStateMachineState.NotStarted;
        }

        public void SynchronizeState(AvailableEnemyState _, AvailableEnemyState current)
        {
            EnemyState newState;
            Debug.Log("Idle" + enemy.IdleState);
            Debug.Log("Chase" + enemy.ChaseState);
            Debug.Log("Attack" + enemy.AttackState);
            Debug.Log("Return" + enemy.ReturnState);
            Debug.Log("Knockback" + enemy.KnockbackState);
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
                case AvailableEnemyState.Return:
                    newState = enemy.ReturnState;
                    break;
                case AvailableEnemyState.None:
                    return;
                default:
                    throw new System.InvalidOperationException("State Synchronization ID Error: " + current);
            }

            if (!IsServer) return;
            if (CurrentEnemyState != null)
                CurrentEnemyState.ExitState();
            CurrentEnemyState = newState;
            CurrentEnemyState.EnterState();
        }
    }
}

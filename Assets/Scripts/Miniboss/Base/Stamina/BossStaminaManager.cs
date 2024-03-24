using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public class BossStaminaManager : NetworkBehaviour
    {
        [Header("Stamina Amount Setting")]
        [SerializeField] public int MaxStamina;
        [SerializeField] public int MinStamina;

        [Header("Stamina Auto Regeneration")]
        [SerializeField] private int staminaRegenInterval;
        [SerializeField] private int autoRegenAmount;

        [Header("Stamina Manual Increment")]
        [SerializeField] private int defaultManualIncrementAmount;
        [SerializeField] private int defaultManualDecrementAmount;
        public NetworkVariable<int> currentStamina = new NetworkVariable<int>();

        private EnemyStateMachine stateMachine;

        public void Awake()
        {
            stateMachine = GetComponent<EnemyStateMachine>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            InvokeRepeating("RegenStamina", 0f, staminaRegenInterval);
            currentStamina.Value = MaxStamina;
            // currentStamina.OnValueChanged += DebugPrintStamina;
            stateMachine.stateMachineState.OnValueChanged += CheckStateMachineState;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (!IsServer) return;
            CancelInvoke("RegenStamina");
            // currentStamina.OnValueChanged -= DebugPrintStamina;
            stateMachine.stateMachineState.OnValueChanged -= CheckStateMachineState;
        }

        public void DebugPrintStamina(int prev, int current)
        {
            Debug.Log("Current Stamina: " + current);
        }

        public void RegenStamina()
        {
            if (!IsServer || currentStamina.Value == MaxStamina) return;
            currentStamina.Value += autoRegenAmount;
        }

        public void DecrementStamina(int value)
        {
            if (!IsServer) return;
            currentStamina.Value = Mathf.Clamp(currentStamina.Value - value, MinStamina, MaxStamina);
        }

        public void DecrementStamina() => DecrementStamina(defaultManualDecrementAmount);

        public void IncrementStamina(int value)
        {
            if (!IsServer) return;
            currentStamina.Value = Mathf.Clamp(currentStamina.Value + value, MinStamina, MaxStamina);
        }

        public void IncrementStamina() => IncrementStamina(defaultManualIncrementAmount);

        public void GenerateStaminaLevelCallback(int targetStaminaLevel, Action callback)
        {
            void _callback(int prev, int current)
            {
                // Callback would also be used if the current level has already reached maxLevel
                if (targetStaminaLevel == current || current == MaxStamina)
                {
                    callback();
                    currentStamina.OnValueChanged -= _callback;
                }
            }

            currentStamina.OnValueChanged += _callback;
        }

        public bool hasEnoughStamina(int requiredStamina) => requiredStamina <= currentStamina.Value;

        // This function returns negative value if current stamina is lower than the target
        public int compareStamina(int target) => target - currentStamina.Value;

        public void CheckStateMachineState(EnemyStateMachine.AvailableStateMachineState prev, EnemyStateMachine.AvailableStateMachineState current)
        {
            if (!IsServer) return;
            if (current == EnemyStateMachine.AvailableStateMachineState.Stopped || 
                current == EnemyStateMachine.AvailableStateMachineState.NotStarted)
            {
                currentStamina.Value = MaxStamina;
            }
        }
    }
}

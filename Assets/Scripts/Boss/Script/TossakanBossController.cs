using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class TossakanBossController : EnemyBase
    {
        [Header("Attack Setup")]
        [SerializeField] public Transform handSpawnSet;
        [SerializeField] public Transform arrowSpawnSet;

        [Header("Phase Setup")]
        [SerializeField] private float phaseTwoThreshold;

        public NetworkVariable<int> currentPhase = new NetworkVariable<int>(1);
        private NetworkVariable<float> reportedHealth = new NetworkVariable<float>(0); // Fake health to report to UI
        private NetworkVariable<bool> isInvincible = new NetworkVariable<bool>(false);

        private float reportedMaxHealth = 0;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            currentHealth.OnValueChanged += CheckHealthForPhaseChange;
            currentHealth.OnValueChanged += UpdateReportedHealth;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            currentHealth.OnValueChanged -= CheckHealthForPhaseChange;
            currentHealth.OnValueChanged -= UpdateReportedHealth;
        }

        private void CheckHealthForPhaseChange(float prev, float current)
        {
            if (!IsServer || current >= phaseTwoThreshold) return;
            StartCoroutine(EnterPhaseTwoCoroutine());
        }

        private void UpdateReportedHealth(float prev, float current)
        {
            if (!IsServer) return;
            if (currentPhase.Value == 1) reportedHealth.Value = current - phaseTwoThreshold;
            else reportedHealth.Value = current;
        }

        public override void Damage(float damageAmount, GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;
            if (dealer.TryGetComponent<EnemyBase>(out EnemyBase enemy)) return; // Prevent Friendly fire

            if (isInvincible.Value) damageAmount = 0; // Boss can become invisible when changing phase

            currentHealth.Value -= damageAmount;
            SpawnDamageFloatingClientRpc(Mathf.Round(damageAmount).ToString());

            if (currentHealth.Value <= 0f)
                Die(dealer);
        }

        private IEnumerator EnterPhaseTwoCoroutine()
        {
            isInvincible.Value = true;
            StateMachine.ChangeState(KnockbackState);
            yield return null;
            // Do Phase 2 Shift Logic here
            // Enter Knockback State
            // Make Boss Invincible
            // Change the Attack State
            // Reset Stamina
            // Reset Health
            // Change reported Max Health
            // etc.
            // Enter Idle State
        }
    }
}

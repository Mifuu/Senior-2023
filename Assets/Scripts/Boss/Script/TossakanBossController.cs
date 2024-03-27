using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace Enemy
{
    public class TossakanBossController : EnemyBase
    {
        [Header("Attack Setup")]
        [SerializeField] public Transform handSpawnSet;
        [SerializeField] public Transform arrowSpawnSet;

        [Header("Second Phase Setup")]
        [SerializeField] private float phaseTwoThreshold;
        [SerializeField] private EnemyAttackSOBase secondPhaseAttackState;
        [SerializeField] private int secondPhaseStamina;

        [Header("Tossakan Spawner Setup")]
        [SerializeField] private OrchestratedSpawnManager tossakanSpawnerRef;

        [Header("Movement Setup")]
        [SerializeField] public GameObject allTossakanPositionAnchor;

        [Header("Stamina")]
        [SerializeField] private BossStaminaManager staminaManager;

        public EnemyBase tossakanPuppet;
        private EnemyAttackSOBase secondPhaseInstance;

        public NetworkVariable<int> currentPhase = new NetworkVariable<int>(1);
        private NetworkVariable<float> reportedHealth = new NetworkVariable<float>(0); // Fake health to report to UI
        private NetworkVariable<bool> isInvincible = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> isChangingPhase = new NetworkVariable<bool>(false);

        private float reportedMaxHealth = 0;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            currentHealth.OnValueChanged += CheckHealthForPhaseChange;
            currentHealth.OnValueChanged += UpdateReportedHealth;
            currentHealth.OnValueChanged += DebugHealth;

            tossakanSpawnerRef.OnEnemySpawns += SetupTossakanDamageable;
            tossakanSpawnerRef.OnEnemySpawns += SetupTossakanAnimationEventEmitter;
            tossakanSpawnerRef.OnEnemySpawns += SetupTossakanPuppet;

            if (secondPhaseAttackState != null)
            {
                var stateInstance = Instantiate(secondPhaseAttackState);
                stateInstance.Initialize(gameObject, this);
                secondPhaseInstance = stateInstance;
            }
            else
                Debug.LogError("Phase two instance in null");

            foreach (Transform t in allTossakanPositionAnchor.transform)
            {
                t.LookAt(targetPlayer.transform);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            currentHealth.OnValueChanged -= CheckHealthForPhaseChange;
            currentHealth.OnValueChanged -= UpdateReportedHealth;
            currentHealth.OnValueChanged -= DebugHealth;
            tossakanSpawnerRef.OnEnemySpawns -= SetupTossakanDamageable;
            tossakanSpawnerRef.OnEnemySpawns -= SetupTossakanAnimationEventEmitter;
            tossakanSpawnerRef.OnEnemySpawns -= SetupTossakanPuppet;
        }

        private void DebugHealth(float prev, float current)
        {
            Debug.LogWarning("Current Health: " + current);
        }

        private void CheckHealthForPhaseChange(float prev, float current)
        {
            if (!IsServer || current >= phaseTwoThreshold) return;
            EnteringPhaseTwoSetup();
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
            if (currentPhase.Value == 1) currentHealth.Value = Mathf.Clamp(currentHealth.Value - damageAmount, phaseTwoThreshold, maxHealth);
            else currentHealth.Value -= damageAmount;

            SpawnDamageFloatingClientRpc(Mathf.Round(damageAmount).ToString());

            if (currentHealth.Value <= 0f)
                Die(dealer);
        }

        private void EnteringPhaseTwoSetup()
        {
            isChangingPhase.Value = true;
            isInvincible.Value = true;

            StateMachine.ChangeState(KnockbackState);
            EnemyAttackBaseInstance = secondPhaseInstance;
            staminaManager.ResetStamina(true, secondPhaseStamina);
        }

        public void OnPhaseChangeAnimationFinished()
        {
            currentPhase.Value = 2;
            isChangingPhase.Value = false;
            isInvincible.Value = false;

            UpdateReportedHealth(0, currentHealth.Value);
            StateMachine.ChangeState(IdleState);
        }

        private void SetupTossakanDamageable(List<EnemyBase> enemyList)
        {
            foreach (var enemy in enemyList)
            {
                var hitboxList = enemy.GetComponentsInChildren<TossakanHitboxDamageable>();
                foreach (var hitbox in hitboxList)
                    hitbox.Initialize(this, dealerPipeline);
            }
        }

        private void SetupTossakanAnimationEventEmitter(List<EnemyBase> enemyList)
        {
            if (enemyList.Count > 1) Debug.LogWarning("There should only be one enemy here (Tossakan) but there are " + enemyList.Count);
            foreach (var enemy in enemyList)
                animationEventEmitter = enemy.GetComponentInChildren<EnemyModelAnimationEventEmitter>();
        }

        private void SetupTossakanPuppet(List<EnemyBase> enemyList)
        {
            if (enemyList.Count > 1) Debug.LogWarning("There should only be one enemy here (Tossakan) but there are " + enemyList.Count);
            foreach (var enemy in enemyList)
                tossakanPuppet = enemy;
        }
    }
}

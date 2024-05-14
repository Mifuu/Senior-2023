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
        [SerializeField] private int secondPhaseStamina;
        [SerializeField] private EnemyIdleSOBase secondPhaseIdleState;
        [SerializeField] private EnemyChaseSOBase secondPhaseChaseState;
        [SerializeField] private EnemyAttackSOBase secondPhaseAttackState;

        [Header("Tossakan Spawner Setup")]
        [SerializeField] private OrchestratedSpawnManager tossakanSpawnerRef;

        [Header("Movement Setup")]
        [SerializeField] public GameObject allTossakanPositionAnchor;

        [Header("Stamina")]
        [SerializeField] private BossStaminaManager staminaManager;

        [Header("Level Setup")]
        [SerializeField] private int startingLevel;

        public EnemyBase tossakanPuppet;
        private EnemyAttackSOBase secondPhaseAttackInstance;
        private EnemyChaseSOBase secondPhaseChaseInstance;
        private EnemyIdleSOBase secondPhaseIdleInstance;

        public NetworkVariable<int> currentPhase = new NetworkVariable<int>(1);
        private NetworkVariable<float> reportedHealth = new NetworkVariable<float>(0); // Fake health to report to UI
        private NetworkVariable<bool> isInvincible = new NetworkVariable<bool>(false);
        public NetworkVariable<bool> isChangingPhase = new NetworkVariable<bool>(false);

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
                secondPhaseAttackInstance = stateInstance;
            }
            else
                Debug.LogError("Phase two attack instance in null");

            if (secondPhaseIdleState != null)
            {
                var stateInstance = Instantiate(secondPhaseIdleState);
                stateInstance.Initialize(gameObject, this);
                secondPhaseIdleInstance = stateInstance;
            }
            else
                Debug.LogError("Phase two idle instance in null");

            if (secondPhaseChaseState != null)
            {
                var stateInstance = Instantiate(secondPhaseChaseState);
                stateInstance.Initialize(gameObject, this);
                secondPhaseChaseInstance = stateInstance;
            }
            else
                Debug.LogError("Phase two attack instance in null");

            foreach (Transform t in allTossakanPositionAnchor.transform)
            {
                t.LookAt(targetPlayer.transform);
            }

            stat.Level.Value = startingLevel;
            GameplayUIBossHP.instance.OpenHealthBar(reportedHealth, phaseTwoThreshold, "Tossakan");
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
            GameplayUIBossHP.instance.CloseHealthBar();
        }

        private void DebugHealth(float prev, float current)
        {
            Debug.LogWarning("Current Health: " + current);
        }

        private void CheckHealthForPhaseChange(float prev, float current)
        {
            if (!IsServer || current > phaseTwoThreshold) return;
            if (currentPhase.Value == 2) return;
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

            if (currentHealth.Value <= 1f)
            {
                Debug.Log("Boss Dead");
                GameplayUI.GameplayUIController.Instance.GameoverTrigger();
                Die(dealer);
            }
        }

        private void EnteringPhaseTwoSetup()
        {
            Debug.Log("Setting up for phase 2");
            isChangingPhase.Value = true;
            isInvincible.Value = true;

            StateMachine.ChangeState(KnockbackState);
            EnemyAttackBaseInstance = secondPhaseAttackInstance;
            EnemyIdleBaseInstance = secondPhaseIdleInstance;
            EnemyChaseBaseInstance = secondPhaseChaseInstance;
            staminaManager.ResetStamina(true, secondPhaseStamina);

            animationEventEmitter.OnPhaseChangeAnimationEnds += OnPhaseChangeAnimationFinished;
        }

        public void OnPhaseChangeAnimationFinished()
        {
            Debug.LogError("ENTERING PHASE 2");
            Debug.LogError("ENTERING PHASE 2");
            Debug.LogError("ENTERING PHASE 2");
            Debug.LogError("ENTERING PHASE 2");
            Debug.LogError("ENTERING PHASE 2");
            Debug.LogError("ENTERING PHASE 2");

            animationEventEmitter.OnPhaseChangeAnimationEnds -= OnPhaseChangeAnimationFinished;

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

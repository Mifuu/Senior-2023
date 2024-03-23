using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System;

namespace Enemy
{
    [RequireComponent(typeof(EnemyStat))]
    public class EnemyBase : NetworkBehaviour, IDamageable
    {
        [Header("Preset Value")]
        [SerializeField] private EnemyTriggerCheck aggroDistanceTriggerCheck;
        public NetworkVariable<float> networkMaxHealth = new NetworkVariable<float>(0f);
        [field: SerializeField] public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized
        [SerializeField] private float maxDistanceSquared;

        #region State ScriptableObject Variable

        [Header("State Machine Behaviour")]
        [SerializeField] private Enemy.EnemyAttackSOBase EnemyAttackBase;
        [SerializeField] private Enemy.EnemyIdleSOBase EnemyIdleBase;
        [SerializeField] private Enemy.EnemyChaseSOBase EnemyChaseBase;
        [SerializeField] private Enemy.EnemyKnockbackSOBase EnemyKnockbackBase;

        public Enemy.EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
        public Enemy.EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
        public Enemy.EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
        public Enemy.EnemyKnockbackSOBase EnemyKnockbackBaseInstance { get; set; }

        #endregion

        #region State Machine Variable

        public EnemyStateMachine StateMachine { get; set; }
        public Enemy.EnemyIdleState IdleState { get; set; }
        public Enemy.EnemyChaseState ChaseState { get; set; }
        public Enemy.EnemyAttackState AttackState { get; set; }
        public Enemy.EnemyKnockbackState KnockbackState { get; set; }

        #endregion

        [Header("Parameter Init at Runtime")]
        public GameObject targetPlayer;
        public NavMeshAgent navMeshAgent;
        public DamageCalculationComponent dealerPipeline;
        public EnemyStat stat;
        private bool initialSetupComplete = false;
        public Rigidbody rigidBody { get; set; }

        [Header("Adjustable Parameter")]
        [Range(0f, 10f)]
        [Tooltip("Configure How fast the Navmesh Agent is turning")]
        [SerializeField] private float navMeshAngularSpeedFactor = 5.0f;
        [Range(0f, 10f)]
        [Tooltip("Configure Navmesh Agent's acceleration")]
        [SerializeField] private float navMeshAcceleration = 2.0f;
        public EnemyModelAnimationEventEmitter animationEventEmitter;
        public event Action OnEnemyDie;

        #region Animation

        [HideInInspector] public Animator animator;
        public readonly int startChasingAnimationTrigger = Animator.StringToHash("StartChasing");
        public readonly int knockedbackAnimationTrigger = Animator.StringToHash("KnockedBack");
        public readonly int attackAnimationTrigger = Animator.StringToHash("Attack");
        public readonly int finishedAttackingAnimationTrigger = Animator.StringToHash("FinishedAttacking");
        public readonly int finishedKnockbackAnimationTrigger = Animator.StringToHash("FinishedKnockback");

        #endregion

        #region VFX

        [Header("VFX")]
        public DamageFloatingSpawner damageFloatingSpawner;

        #endregion

        public void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            StateMachine = GetComponent<EnemyStateMachine>();
            dealerPipeline = GetComponent<DamageCalculationComponent>();
            stat = GetComponent<EnemyStat>();
            animator = GetComponentInChildren<Animator>();

            navMeshAgent.angularSpeed = navMeshAngularSpeedFactor * navMeshAgent.angularSpeed;
            navMeshAgent.acceleration = navMeshAgent.acceleration * navMeshAcceleration;
        }

        public void Update()
        {
            StateMachine.CurrentEnemyState.FrameUpdate();
        }

        public void FixedUpdate()
        {
            StateMachine.CurrentEnemyState.PhysicsUpdate();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!initialSetupComplete)
            {
                initialSetupComplete = true;
                if (IsServer) ServerSetup();
                else ClientSetup();
            }

            if (IsServer)
                networkMaxHealth.Value = maxHealth;

            networkMaxHealth.OnValueChanged += AdjustMaxHealth;
            OnEnemySpawn();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            networkMaxHealth.OnValueChanged -= AdjustMaxHealth;
        }

        public override void OnDestroy()
        {
            if (IsServer) ServerDesetup();
        }

        private void ClientSetup()
        {
            // Debug.Log(gameObject + " Running Non Server Setup");
            enabled = false;
            StateMachine.enabled = false;
            Destroy(navMeshAgent);
            // Destroy(GetComponent<NetworkRigidbody>());
            // Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
        }

        private void ServerSetup()
        {
            // Debug.Log(gameObject + " Running Server Setup");
            EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
            EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);
            EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
            EnemyKnockbackBaseInstance = Instantiate(EnemyKnockbackBase);

            IdleState = new Enemy.EnemyIdleState(this, StateMachine);
            ChaseState = new Enemy.EnemyChaseState(this, StateMachine);
            AttackState = new Enemy.EnemyAttackState(this, StateMachine);
            KnockbackState = new Enemy.EnemyKnockbackState(this, StateMachine);

            EnemyIdleBaseInstance.Initialize(gameObject, this);
            EnemyAttackBaseInstance.Initialize(gameObject, this);
            EnemyChaseBaseInstance.Initialize(gameObject, this);
            EnemyKnockbackBaseInstance.Initialize(gameObject, this);

            StateMachine.Initialize(IdleState);

            rigidBody.isKinematic = false;
            rigidBody.useGravity = false;

            aggroDistanceTriggerCheck.OnHitboxTriggerEnter += ChangeTargetPlayerFromCollision;
        }

        private void ServerDesetup()
        {
            aggroDistanceTriggerCheck.OnHitboxTriggerEnter -= ChangeTargetPlayerFromCollision;
        }

        private void OnEnemySpawn()
        {
            if (!IsServer) return;
            OnTargetPlayerRefindRequired();
            currentHealth.Value = maxHealth;
            // StateMachine.ChangeState(IdleState);
        }

        public void Damage(float damageAmount, GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;
            if (dealer.TryGetComponent<EnemyBase>(out EnemyBase enemy)) return; // Prevent Friendly fire

            currentHealth.Value -= damageAmount;
            SpawnDamageFloatingClientRpc(Mathf.Round(damageAmount).ToString());

            if (currentHealth.Value <= 0f)
                Die(dealer);
        }

        [ClientRpc]
        private void SpawnDamageFloatingClientRpc(string value)
        {
            if (damageFloatingSpawner != null)
                damageFloatingSpawner.Spawn(value);
        }

        public void Die(GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;

            if (dealer != null) 
                dealer.GetComponent<PlayerLevel>()?.AddExp(stat.BaseEXP.Value); 

            CleanUp();
            OnEnemyDie?.Invoke();

            if (TryGetComponent<NetworkObject>(out var enemyNetworkObject))
                enemyNetworkObject.Despawn();
        }

        private void CleanUp()
        {
            // Place for more clean up logic, animation etc.
            DesetupTargetPlayer();
        }

        private void AnimationTrigger(int triggerType)
        {
            StateMachine.CurrentEnemyState.AnimationTrigger(triggerType);
        }

        // Test animation trigger type - May not really be used
        // public enum AnimationTriggerType
        // {
        //     EnemyDamaged,
        //     PlayFootstepSounds
        // }

        private void ChangeTargetPlayerFromCollision(Collider other) => OnTargetPlayerChangeRequired(other.gameObject);

        private bool CheckIsNewPlayer(GameObject objectToCheck) => objectToCheck.GetComponent<PlayerHealth>() != null && objectToCheck != targetPlayer;

        private GameObject FindTargetPlayer()
        {
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = null;
            float closestDistanceSqr = float.MaxValue;

            foreach (var player in allPlayers)
            {
                if (!CheckIsNewPlayer(player)) continue;

                float distanceSqr = (player.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestPlayer = player;
                    closestDistanceSqr = distanceSqr;
                }
            }

            if (closestDistanceSqr > maxDistanceSquared) return null; // Allow Enemy to search only to a certain distance
            return closestPlayer;
        }

        public void OnTargetPlayerChangeRequired(GameObject newTargetPlayer)
        {
            if (!IsServer || newTargetPlayer == null || !CheckIsNewPlayer(newTargetPlayer)) return;
            if (targetPlayer != null)
            {
                DesetupTargetPlayer();
            }

            targetPlayer = newTargetPlayer;
            SetupNewTargetPlayer(targetPlayer);
        }

        private void OnTargetPlayerRefindRequired()
        {
            if (!IsServer) return;
            if (targetPlayer != null)
            {
                DesetupTargetPlayer();
            }

            var newPlayer = FindTargetPlayer();
            if (newPlayer == null)
            {
                Die(null);
                return;
            }

            SetupNewTargetPlayer(newPlayer);
        }

        private void DesetupTargetPlayer()
        {
            if (targetPlayer != null && targetPlayer.TryGetComponent<PlayerHealth>(out var playerHealth))
                playerHealth.OnPlayerDie -= OnTargetPlayerRefindRequired;
            targetPlayer = null;
        }

        private void SetupNewTargetPlayer(GameObject newTargetPlayer)
        {
            targetPlayer = newTargetPlayer;
            ChangeTargetPlayerClientRpc(newTargetPlayer.GetComponent<NetworkObject>());

            // Setup target player, such as subscribe to player die event
            if (targetPlayer.TryGetComponent<PlayerHealth>(out var playerHealth))
                playerHealth.OnPlayerDie += OnTargetPlayerRefindRequired;
        }

        private void AdjustMaxHealth(float prev, float current) => maxHealth = current;

        [ClientRpc]
        private void ChangeTargetPlayerClientRpc(NetworkObjectReference targetPlayerRef)
        {
            if (targetPlayerRef.TryGet(out NetworkObject targetPlayerNetworkObj, NetworkManager.Singleton))
            {
                targetPlayer = targetPlayerNetworkObj.gameObject;
            }
            else
            {
                Debug.LogError("Target Player Not found");
            }
        }
    }
}

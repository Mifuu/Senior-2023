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
        [SerializeField] private float maxDistanceSquared = 5000f;
        [field: SerializeField] public float maxHealth { get; set; }

        public NetworkVariable<float> networkMaxHealth = new NetworkVariable<float>(0f);
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized

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
        private ulong targetPlayerNetworkId;

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

        #endregion

        #region VFX

        [Header("VFX")]
        public DamageFloatingSpawner damageFloatingSpawner;

        #endregion

        #region Sound Controller Setup For Spawn and Dead Sound

        [Header("Sound Controller Setup")]
        [SerializeField] public string soundListName;
        [SerializeField] public string soundSpawnName;
        [SerializeField] public string soundDeadName;

        public EnemyAudioControllerSingular audioController;

        #endregion

        public void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            StateMachine = GetComponent<EnemyStateMachine>();
            dealerPipeline = GetComponent<DamageCalculationComponent>();
            stat = GetComponent<EnemyStat>();
            animator = GetComponentInChildren<Animator>();

            if (rigidBody == null || navMeshAgent == null || StateMachine == null ||
                    dealerPipeline == null || stat == null || animator == null)
                Debug.LogError($"Check {gameObject} Config");

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
            audioController = EnemyAudioController.Singleton.Get(soundListName);
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

            if (rigidBody != null)
            {
                rigidBody.isKinematic = false;
                rigidBody.useGravity = false;
            }

            aggroDistanceTriggerCheck.OnHitboxTriggerEnter += ChangeTargetPlayerFromCollision;
        }

        private void ServerDesetup()
        {
            aggroDistanceTriggerCheck.OnHitboxTriggerEnter -= ChangeTargetPlayerFromCollision;
            CleanUp();
        }

        private void OnEnemySpawn()
        {
            if (!IsServer) return;
            audioController?.PlaySFXAtObject(soundSpawnName, transform.position);
            OnTargetPlayerRefindRequired();
            currentHealth.Value = maxHealth;
            // StateMachine.ChangeState(IdleState);
        }

        public virtual void Damage(float damageAmount, GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;
            if (dealer != null)
                if (dealer.TryGetComponent<EnemyBase>(out EnemyBase enemy)) return; // Prevent Friendly fire

            currentHealth.Value -= damageAmount;
            SpawnDamageFloatingClientRpc(Mathf.Round(damageAmount).ToString());

            if (currentHealth.Value <= 0f)
                Die(dealer);
        }

        [ClientRpc]
        protected void SpawnDamageFloatingClientRpc(string value)
        {
            if (damageFloatingSpawner != null)
                damageFloatingSpawner.Spawn(value);
        }

        public void Die(GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;

            if (dealer != null)
                dealer.GetComponent<PlayerLevel>()?.AddExp(stat.BaseEXP.Value);

            audioController?.PlaySFXAtObject(soundDeadName, transform.position);
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

        private void ChangeTargetPlayerFromCollision(Collider other) => OnTargetPlayerChangeRequired(other.gameObject);

        private bool CheckIsNewPlayer(GameObject objectToCheck) => objectToCheck.GetComponent<PlayerHealth>() != null && objectToCheck != targetPlayer;

        private GameObject FindTargetPlayer()
        {
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = null;
            float closestDistanceSqr = float.MaxValue;

            Debug.LogError("All Players: " + allPlayers.Length + "");

            foreach (var player in allPlayers)
            {
                if (!CheckIsNewPlayer(player)) continue;

                float distanceSqr = (player.transform.position - transform.position).sqrMagnitude;
                Debug.Log("Distance: " + distanceSqr + "");
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestPlayer = player;
                    closestDistanceSqr = distanceSqr;
                }
            }
            // Allow Enemy to search only to a certain distance
            // if (closestDistanceSqr > maxDistanceSquared) return null;
            Debug.LogError("Closest Player: " + closestPlayer + "," + closestDistanceSqr + "," + maxDistanceSquared + "");
            return closestPlayer;
        }

        public void OnTargetPlayerChangeRequired(GameObject newTargetPlayer)
        {
            if (!IsServer || newTargetPlayer == null || !CheckIsNewPlayer(newTargetPlayer)) return;
            if (targetPlayer != null)
                DesetupTargetPlayer();

            targetPlayer = newTargetPlayer;
            SetupNewTargetPlayer(targetPlayer);
        }

        private void OnTargetPlayerRefindRequired()
        {
            if (!IsServer) return;
            if (targetPlayer != null)
                DesetupTargetPlayer();

            var newPlayer = FindTargetPlayer();

            if (newPlayer == null)
            {
                Debug.LogError("New Player is Null ");
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

        private void HandleClientDisconnect(ulong playerID)
        {
            if (playerID == targetPlayerNetworkId)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
                OnTargetPlayerRefindRequired();
            }
        }

        private void SetupNewTargetPlayer(GameObject newTargetPlayer)
        {
            if (newTargetPlayer.TryGetComponent<PlayerHealth>(out var playerHealth) && newTargetPlayer.TryGetComponent<NetworkObject>(out var networkObject))
            {
                targetPlayer = newTargetPlayer;
                playerHealth.OnPlayerDie += OnTargetPlayerRefindRequired;
                ChangeTargetPlayerClientRpc(networkObject);
                targetPlayerNetworkId = networkObject.NetworkObjectId;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
            }
            else
            {
                Debug.LogError("Target Player Setup Error");
                Die(null);
            }
        }

        private void AdjustMaxHealth(float prev, float current) => maxHealth = current;

        [ClientRpc]
        private void ChangeTargetPlayerClientRpc(NetworkObjectReference targetPlayerRef)
        {
            if (targetPlayerRef.TryGet(out NetworkObject targetPlayerNetworkObj, NetworkManager.Singleton))
                targetPlayer = targetPlayerNetworkObj.gameObject;
            else
                Debug.LogError("Target Player Not found");
        }
    }
}

using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System;
using System.Collections;

namespace Enemy
{
    [RequireComponent(typeof(EnemyStat))]
    public class EnemyBase : NetworkBehaviour, IDamageable
    {
        #region Distance Check

        [Header("Choose Target Player Distance")]
        [SerializeField] private bool disabledDistanceCheck;
        [SerializeField] private EnemyTriggerCheck aggroDistanceTriggerCheck;
        [Tooltip("Max Distance the enemy can target")]
        [SerializeField] private float newTargetPlayerMaxDistanceSqr = 300f;

        #endregion

        #region Return To Spawn Point 

        [Header("Returning To Spawn Point")]
        [Tooltip("Max Distance from spawn point enemy can go")]
        [SerializeField] private bool disabledReturnState;
        [SerializeField] public float maxDistanceSqrFromSpawnPoint = 300f;
        [SerializeField] private float spawnDistanceCheckTimeInterval = 5f;

        [HideInInspector] public Vector3 pointOfOrigin;
        private IEnumerator spawnDistanceCheckCoroutine;

        #endregion

        #region Health

        [field: SerializeField, Header("Health")]
        public float maxHealth { get; set; }

        public NetworkVariable<float> networkMaxHealth = new NetworkVariable<float>(0f);
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized

        #endregion

        #region State ScriptableObject Variable

        [Header("State Machine Behaviour")]
        [SerializeField] private Enemy.EnemyAttackSOBase EnemyAttackBase;
        [SerializeField] private Enemy.EnemyIdleSOBase EnemyIdleBase;
        [SerializeField] private Enemy.EnemyChaseSOBase EnemyChaseBase;
        [SerializeField] private Enemy.EnemyKnockbackSOBase EnemyKnockbackBase;
        [SerializeField] private Enemy.EnemyReturnSOBase EnemyReturnBase;

        public Enemy.EnemyIdleSOBase EnemyIdleBaseInstance { get; set; }
        public Enemy.EnemyAttackSOBase EnemyAttackBaseInstance { get; set; }
        public Enemy.EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
        public Enemy.EnemyKnockbackSOBase EnemyKnockbackBaseInstance { get; set; }
        public Enemy.EnemyReturnSOBase EnemyReturnBaseInstance { get; set; }

        #endregion

        #region State Machine Variable

        public EnemyStateMachine StateMachine { get; set; }
        public Enemy.EnemyIdleState IdleState { get; set; }
        public Enemy.EnemyChaseState ChaseState { get; set; }
        public Enemy.EnemyAttackState AttackState { get; set; }
        public Enemy.EnemyKnockbackState KnockbackState { get; set; }
        public Enemy.EnemyReturnState ReturnState { get; set; }

        #endregion

        #region Utility

        [Header("Reality Check")]
        public GameObject targetPlayer;
        public NavMeshAgent navMeshAgent;
        public DamageCalculationComponent dealerPipeline;
        public EnemyStat stat;
        public Rigidbody rigidBody { get; set; }

        #endregion

        [Header("Nav Mesh")]
        [Range(0f, 10f)]
        [Tooltip("Configure How fast the Navmesh Agent is turning")]
        [SerializeField] private float navMeshAngularSpeedFactor = 5.0f;

        [Range(0f, 10f)]
        [Tooltip("Configure Navmesh Agent's acceleration")]
        [SerializeField] private float navMeshAcceleration = 2.0f;

        #region Event

        public event Action<GameObject> OnEnemyDie; // Pass the Killer GameObject, Nullable
        public event Action<GameObject> OnTargetPlayerChanged; // Pass the new Targer player, Nullable

        #endregion

        #region Animation

        [HideInInspector] public Animator animator;
        public EnemyModelAnimationEventEmitter animationEventEmitter;

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

        private bool initialSetupComplete = false;
        private ulong targetPlayerNetworkId;

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
            {
                networkMaxHealth.Value = maxHealth;
                currentHealth.Value = maxHealth;
                OnTargetPlayerRefindRequired();
                audioController?.PlaySFXAtObject(soundSpawnName, transform.position);
                spawnDistanceCheckCoroutine = CheckDistanceFromSpawn();
                if (!disabledReturnState && isActiveAndEnabled)
                    StartCoroutine(spawnDistanceCheckCoroutine);
            }

            networkMaxHealth.OnValueChanged += AdjustMaxHealth;
            // StateMachine.ChangeState(IdleState);
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
            EnemyReturnBaseInstance = Instantiate(EnemyReturnBase);

            IdleState = new Enemy.EnemyIdleState(this, StateMachine);
            ChaseState = new Enemy.EnemyChaseState(this, StateMachine);
            AttackState = new Enemy.EnemyAttackState(this, StateMachine);
            KnockbackState = new Enemy.EnemyKnockbackState(this, StateMachine);
            ReturnState = new Enemy.EnemyReturnState(this, StateMachine);

            EnemyIdleBaseInstance.Initialize(gameObject, this);
            EnemyAttackBaseInstance.Initialize(gameObject, this);
            EnemyChaseBaseInstance.Initialize(gameObject, this);
            EnemyKnockbackBaseInstance.Initialize(gameObject, this);
            EnemyReturnBaseInstance.Initialize(gameObject, this);

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
            DesetupTargetPlayer();
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

        public void Die(GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;

            if (dealer != null)
                dealer.GetComponent<PlayerLevel>()?.AddExp(stat.BaseEXP.Value);

            audioController?.PlaySFXAtObject(soundDeadName, transform.position);
            DesetupTargetPlayer();
            OnEnemyDie?.Invoke(dealer);

            if (spawnDistanceCheckCoroutine != null)
            {
                StopCoroutine(spawnDistanceCheckCoroutine);
                spawnDistanceCheckCoroutine = null;
            }

            if (TryGetComponent<NetworkObject>(out var enemyNetworkObject))
                enemyNetworkObject.Despawn();
        }

        [ClientRpc]
        protected void SpawnDamageFloatingClientRpc(string value)
        {
            if (damageFloatingSpawner != null)
                damageFloatingSpawner.Spawn(value);
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
            /* Debug.LogError("All Players: " + allPlayers.Length + ""); */

            foreach (var player in allPlayers)
            {
                if (!CheckIsNewPlayer(player)) continue;

                float distanceSqr = (player.transform.position - transform.position).sqrMagnitude;
                /* Debug.Log("Distance: " + distanceSqr + ""); */
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestPlayer = player;
                    closestDistanceSqr = distanceSqr;
                }
            }
            // Allow Enemy to search only to a certain distance
            if (!disabledDistanceCheck && closestDistanceSqr > newTargetPlayerMaxDistanceSqr) return null;
            /* Debug.LogError("Closest Player: " + closestPlayer + "," + closestDistanceSqr + "," + maxDistanceSquared + ""); */
            return closestPlayer;
        }

        public void OnTargetPlayerChangeRequired(GameObject newTargetPlayer)
        {
            if (!IsServer || newTargetPlayer == null || !CheckIsNewPlayer(newTargetPlayer)) return;
            if (targetPlayer != null)
                DesetupTargetPlayer();

            targetPlayer = newTargetPlayer;
            OnTargetPlayerChanged?.Invoke(newTargetPlayer);
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
                Debug.LogWarning($"New Player is Null, {gameObject} will kill Thyself");
                OnTargetPlayerChanged?.Invoke(newPlayer);
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
                OnTargetPlayerChanged?.Invoke(newTargetPlayer);
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

        private IEnumerator CheckDistanceFromSpawn()
        {
            pointOfOrigin = transform.position;
            if (!IsServer) yield break;

            while (true)
            {
                if ((pointOfOrigin - targetPlayer.transform.position).sqrMagnitude > maxDistanceSqrFromSpawnPoint
                        && StateMachine.networkEnemyState.Value != EnemyStateMachine.AvailableEnemyState.Return)
                    StateMachine.ChangeState(ReturnState);

                yield return new WaitForSeconds(spawnDistanceCheckTimeInterval);
            }
        }

        [ClientRpc]
        private void ChangeTargetPlayerClientRpc(NetworkObjectReference targetPlayerRef)
        {
            if (targetPlayerRef.TryGet(out NetworkObject targetPlayerNetworkObj, NetworkManager.Singleton))
            {
                targetPlayer = targetPlayerNetworkObj.gameObject;
                OnTargetPlayerChanged?.Invoke(targetPlayer);
            }
            else
                Debug.LogError("Target Player Not found");
        }
    }
}

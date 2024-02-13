using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using Unity.Netcode.Components;
using ObserverPattern;

namespace Enemy
{
    public class EnemyBase : NetworkBehaviour, IDamageable
    {
        [field: SerializeField] public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized
        public Rigidbody rigidBody { get; set; }
        [SerializeField] private EnemyTriggerCheck aggroDistanceTriggerCheck;
        private bool initialSetupComplete = false;

        #region State ScriptableObject Variable

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

        public GameObject targetPlayer;
        public NavMeshAgent navMeshAgent;

        #region Stat and Damage Calculation

        public Subject<float> BaseAtk = new Subject<float>(100f);
        public Subject<float> HydroDamageBonus = new Subject<float>(2f);
        public Subject<float> PyroDamageBonus = new Subject<float>(2f);
        public Subject<float> ElectroDamageBonus = new Subject<float>(2f);
        public Subject<float> CritRate = new Subject<float>(0.5f);
        public Subject<float> CritDmgFactor = new Subject<float>(1.2f);

        public DamageDealerCalculationPipeline dealerPipeline;

        #endregion

        public void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            StateMachine = GetComponent<EnemyStateMachine>();
            dealerPipeline = GetComponent<DamageDealerCalculationPipeline>();
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
            if (initialSetupComplete) return;
            initialSetupComplete = true;

            if (!IsServer)
            {
                ClientSetup();
            }
            else
            {
                ServerSetup();
            }

            OnEnemySpawn();
        }

        private void ClientSetup()
        {
            Debug.Log("Running Non Server Setup");
            Destroy(navMeshAgent);
            Destroy(GetComponent<NetworkRigidbody>());
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
            enabled = false;
            StateMachine.enabled = false;
        }

        private void ServerSetup()
        {
            Debug.Log("Running Server Setup");
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

            aggroDistanceTriggerCheck.OnHitboxTriggerEnter += (Collider other) => OnTargetPlayerChangeRequired(other.gameObject);
        }

        private void OnEnemySpawn()
        {
            if (!IsServer) return;
            OnTargetPlayerRefindRequired();
            currentHealth.Value = maxHealth;
            StateMachine.ChangeState(IdleState);
        }

        public void Damage(float damageAmount, GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;
            currentHealth.Value -= damageAmount;
            if (currentHealth.Value <= 0f)
            {
                Die(dealer);
            }
        }

        public void Die(GameObject dealer)
        {
            if (!IsServer || !isActiveAndEnabled) return;
            dealer.GetComponent<PlayerLevel>()?.AddExp(100);
            CleanUp();
            var enemyNetworkObject = GetComponent<NetworkObject>();
            enemyNetworkObject.Despawn();
            // NetworkObjectPool.Singleton.ReturnNetworkObject(enemyNetworkObject, gameObject);
        }

        private void CleanUp()
        {
            // Place for more clean up logic, animation etc.
            DesetupTargetPlayer();
        }

        private void AnimationTrigger(AnimationTriggerType triggerType)
        {
            StateMachine.CurrentEnemyState.AnimationTrigger(triggerType);
        }

        // Test animation trigger type - May not really be used
        public enum AnimationTriggerType
        {
            EnemyDamaged,
            PlayFootstepSounds
        }

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
            return closestPlayer;
        }

        public void OnTargetPlayerChangeRequired(GameObject newTargetPlayer)
        {
            if (!IsServer) return;
            if (targetPlayer != null)
            {
                DesetupTargetPlayer();
            }
            if (!CheckIsNewPlayer(newTargetPlayer)) return;

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
            var playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            playerHealth.OnPlayerDie -= OnTargetPlayerRefindRequired;
        }

        private void SetupNewTargetPlayer(GameObject newTargetPlayer)
        {
            targetPlayer = newTargetPlayer;
            ChangeTargetPlayerClientRpc(newTargetPlayer.GetComponent<NetworkObject>());

            // Setup target player, such as subscribe to player die event
            var playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            playerHealth.OnPlayerDie += OnTargetPlayerRefindRequired;
        }

        [ClientRpc]
        private void ChangeTargetPlayerClientRpc(NetworkObjectReference targetPlayerRef)
        {
            if (targetPlayerRef.TryGet(out NetworkObject targetPlayerNetworkObj, NetworkManager.Singleton))
            {
                Debug.Log(gameObject + " has new Target Player: " + targetPlayerNetworkObj);
                targetPlayer = targetPlayerNetworkObj.gameObject;
            }
            else
            {
                Debug.LogError("Target Player Not found");
            }
        }
    }
}

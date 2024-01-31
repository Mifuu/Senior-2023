using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using Unity.Netcode.Components;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyBase : NetworkBehaviour, IDamageable
    {
        [field: SerializeField] public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized
        public Rigidbody rigidBody { get; set; }

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

        public void Start()
        {
            EnemyIdleBaseInstance.Initialize(gameObject, this);
            EnemyAttackBaseInstance.Initialize(gameObject, this);
            EnemyChaseBaseInstance.Initialize(gameObject, this);
            EnemyKnockbackBaseInstance.Initialize(gameObject, this);

            StateMachine.Initialize(IdleState);
        }

        public void Awake()
        {
            EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
            EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);
            EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
            EnemyKnockbackBaseInstance = Instantiate(EnemyKnockbackBase);

            StateMachine = gameObject.AddComponent<EnemyStateMachine>();

            IdleState = new Enemy.EnemyIdleState(this, StateMachine);
            ChaseState = new Enemy.EnemyChaseState(this, StateMachine);
            AttackState = new Enemy.EnemyAttackState(this, StateMachine);
            KnockbackState = new Enemy.EnemyKnockbackState(this, StateMachine);

            rigidBody = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
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
            OnEnemySpawn();
            if (!IsServer)
            {
                Destroy(navMeshAgent);
                Destroy(GetComponent<NetworkRigidbody>());
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<Collider>());
            }
        }

        public override void OnNetworkDespawn()
        {
            var playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            playerHealth.OnPlayerDie -= OnTargetPlayerRefindRequired;
        }

        private void OnEnemySpawn()
        {
            if (!IsServer) return; // temporary fix
            OnTargetPlayerRefindRequired();
            currentHealth.Value = maxHealth;
            StateMachine.ChangeState(IdleState);
        }

        public void Damage(float damageAmount)
        {
            if (!IsServer) return;
            currentHealth.Value -= damageAmount;
            if (currentHealth.Value <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            CleanUp();
            var enemyNetworkObject = GetComponent<NetworkObject>();
            enemyNetworkObject.Despawn();
            // BUG (FATAL): GameObject can not be used in the ReturnNetworkObject function, must reference actual prefab
            // Comment this out as a temporary fix
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

        private GameObject FindTargetPlayer()
        {
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = null;
            float closestDistanceSqr = float.MaxValue;

            foreach (var player in allPlayers)
            {
                float distanceSqr = (player.transform.position - transform.position).sqrMagnitude;
                if (distanceSqr <= closestDistanceSqr)
                {
                    closestPlayer = player;
                    closestDistanceSqr = distanceSqr;
                }
            }
            return closestPlayer;
        }

        private void OnTargetPlayerChangeRequired(GameObject newTargetPlayer)
        {
            if (!IsServer) return;
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
            if (newPlayer == null && IsServer)
            {
                Die();
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
                Debug.Log("New Target Player: " + targetPlayerNetworkObj);
                targetPlayer = targetPlayerNetworkObj.gameObject;
            }
            else
            {
                Debug.LogError("Target Player Not found");
            }
        }
    }
}

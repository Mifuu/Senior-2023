using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : NetworkBehaviour, IDamageable
    {
        // TODO: Find a way to make everything works in NETCODE
        // TODO: Implement IPlayerTargettable which must have Targetplayer and Ontargetplayerchange function
        // CONTINUE HERE,
        public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0.0f); // NetworkVariable must be initialized
        public Rigidbody rigidBody { get; set; }
        public event Action<GameObject> OnTargetPlayerDie; // Pass the new player game object or null to the subscriber

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

        #region Player Variable

        public GameObject targetPlayer;
        public event Action<GameObject> OnTargetPlayerChange;

        #endregion

        public void Start()
        {
            rigidBody = GetComponent<Rigidbody>();

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

            StateMachine = new EnemyStateMachine();

            IdleState = new Enemy.EnemyIdleState(this, StateMachine);
            ChaseState = new Enemy.EnemyChaseState(this, StateMachine);
            AttackState = new Enemy.EnemyAttackState(this, StateMachine);
            KnockbackState = new Enemy.EnemyKnockbackState(this, StateMachine);
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
            // TODO: Maybe move this so that it also works when to when the player dies and has to find new player as well
            // TODO: Make a subscription to player dying event maybe
            targetPlayer = FindTargetPlayer();
            currentHealth.Value = maxHealth;
        }

        #region Enemy Damageable Logic

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
            var enemyNetworkObject = GetComponent<NetworkObject>();
            enemyNetworkObject.Despawn();
            // BUG (FATAL): GameObject can not be used in the ReturnNetworkObject function, must reference actual prefab
            // Comment this out as a temporary fix
            // NetworkObjectPool.Singleton.ReturnNetworkObject(enemyNetworkObject, gameObject);
        }

        private void CleanUp()
        {
            StateMachine.ChangeState(IdleState);
            currentHealth.Value = maxHealth;
            // Place for more clean up logic, animation etc.
        }

        #endregion

        #region Animation

        private void AnimationTrigger(AnimationTriggerType triggerType)
        {
            StateMachine.CurrentEnemyState.AnimationTrigger(triggerType);
        }

        #endregion

        #region Coroutine Utility

        public Coroutine PerformCoroutine(IEnumerator ienumerator)
        {
            return StartCoroutine(ienumerator);
        }

        public void PerformStopCoroutine(IEnumerator ienumerator)
        {
            StopCoroutine(ienumerator);
        }

        #endregion

        // Test animation trigger type - May not really be used
        public enum AnimationTriggerType
        {
            EnemyDamaged,
            PlayFootstepSounds
        }

        // TODO: Create logic for when the player died, and the target player change
        #region Player Logic

#nullable enable
        private GameObject? FindTargetPlayer()
        {
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            GameObject? closestPlayer = null;
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

        #endregion
    }
}

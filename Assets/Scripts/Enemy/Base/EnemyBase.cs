using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : NetworkBehaviour, IDamageable, ITriggerCheckable
    {
        // TODO: Find a way to make everything works in NETCODE
        // TODO: Remove unnecessary stuff
        public float maxHealth { get; set; }
        public NetworkVariable<float> currentHealth { get; set; } = new NetworkVariable<float>(0); // NetworkVariable must be initialized
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

        #region Trigger Variable

        public bool isWithinStrikingDistance { get; set; }
        public event Action<Collision> OnEnemyCollide;
        public event Action<Collider> OnEnemyTrigger;

        #endregion

        #region Player Interaction Variable

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
            // TODO: Maybe move this so that it also works when to when the player dies as well
            // TODO: Make a subscription to player dying event maybe
            targetPlayer = FindTargetPlayer();
            currentHealth.Value = maxHealth;
        }

        #region Enemy Damage and Die Logic

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
            StateMachine.ChangeState(IdleState);
            currentHealth.Value = maxHealth;

            var enemyNetworkObject = GetComponent<NetworkObject>();
            enemyNetworkObject.Despawn();
            // BUG (FATAL): GameObject can not be used in the ReturnNetworkObject function, must reference actual prefab
            NetworkObjectPool.Singleton.ReturnNetworkObject(enemyNetworkObject, gameObject);
        }

        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log(OnEnemyCollide);
            OnEnemyCollide?.Invoke(collision);
        }

        public void OnTriggerEnter(Collider collider)
        {
            Debug.Log(OnEnemyTrigger);
            OnEnemyTrigger?.Invoke(collider);
        }

        #endregion

        #region Animation

        private void AnimationTrigger(AnimationTriggerType triggerType)
        {
            StateMachine.CurrentEnemyState.AnimationTrigger(triggerType);
        }

        #endregion

        #region Trigger Check 

        public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
        {
            this.isWithinStrikingDistance = isWithinStrikingDistance;
        }

        #endregion

        #region Attack

        public void PerformAttack(EnemyAttack attack)
        {
            attack.PerformAttack();
        }

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

        // TODO: Create logic for when the player died, or the target would change
        #region Player Interaction Logic

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

using UnityEngine;
using Unity.Netcode;
using System;

namespace Enemy
{
    public abstract class EnemyAOEBase : NetworkBehaviour
    {
        [SerializeField] protected GameObject AOEGameObjectPrefab;
        public GameObject PlayerTarget { get; set; }
        public EnemyBase enemy { get; set; }

        [Header("Temporary Material Field")]
        [SerializeField] private Material activateMaterial;
        [SerializeField] private Material preActivateMaterial;
        private NetworkVariable<bool> isOnActivateMaterial = new NetworkVariable<bool>(false);

        protected EnemyWithinTriggerCheck areaOfEffectTrigger;
        public event Action OnAOEPeriodEnd;
        protected DamageCalculationComponent component;

        #region Animation

        protected Animator animator;
        protected readonly int attackAnimationTrigger = Animator.StringToHash("BeginAttack");
        protected readonly int endAOEAnimationTrigger = Animator.StringToHash("BeginEnd");

        #endregion

        public void Awake()
        {
            areaOfEffectTrigger = transform.Find("AOE")?.GetComponent<EnemyWithinTriggerCheck>();
            animator = GetComponent<Animator>();
            animator.keepAnimatorStateOnDisable = false;

            if (areaOfEffectTrigger == null)
                Debug.LogError("AOE have no trigger check (Hitbox)");
        }

        public virtual void InitializeAOE(GameObject PlayerTarget, EnemyBase enemy, DamageCalculationComponent component = null)
        {
            // Debug.Log("AOE Base Player Target: " + PlayerTarget);
            this.PlayerTarget = PlayerTarget;
            this.enemy = enemy;
            if (component == null)
                this.component = enemy.dealerPipeline;
            else
                this.component = component;
        }

        protected void EmitAOEEndsEvent()
        {
            OnAOEPeriodEnd?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ResetValue();
        }

        public virtual void PreEffect() { }
        public virtual void CancelPreEffect() { }
        public virtual void ActivateEffect()
        {
            if (!IsServer) return;
            animator.SetTrigger(attackAnimationTrigger);
        }
        public virtual void CancelEffect() { }
        public virtual void ResetValue()
        {
            this.PlayerTarget = null;
            this.enemy = null;
            if (!IsServer) return;
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Enemy
{
    // <summary>
    // Enemy Attack is the base of all the attack patterns of the enemy
    // NOTE : This does not derive from Monobehaviour, do not attach this to gameobject
    // Attach this to Attack Enemy State Scriptable Object
    // </summary>
    public abstract class EnemyAttack : ScriptableObject
    {
        protected EnemyBase enemy;
        protected DamageCalculationComponent damageComponent;
        protected GameObject enemyGameObject;
        public event Action OnAttackEnds;
        public HashSet<IDamageCalculatable> processedDamagable;

        [Header("Animation")]
        [SerializeField] private bool setPerformAttack;
        [SerializeField] private string performAttackTriggerName;
        private int performAttackTrigger;

        public virtual void Initialize(GameObject targetPlayer, GameObject enemyGameObject, DamageCalculationComponent component = null)
        {
            this.enemy = enemyGameObject.GetComponent<EnemyBase>();
            this.enemyGameObject = enemyGameObject;
            if (component == null)
                this.damageComponent = enemyGameObject.GetComponent<DamageCalculationComponent>();
            else 
                this.damageComponent = component;
            ResetProcessedDamageable();
            performAttackTrigger = Animator.StringToHash(performAttackTriggerName);
        }

        public DamageInfo Damage(IDamageCalculatable damageable)
        {
            DamageInfo info = damageComponent.GetFinalDealthDamageInfo();
            if (damageable != null)
            {
                if (processedDamagable.Contains(damageable)) return info;
                damageable.Damage(info);
                processedDamagable.Add(damageable);
            }

            return info;
        }

        protected void EmitAttackEndsEvent()
        {
            OnAttackEnds?.Invoke();
        }

        protected void ResetProcessedDamageable()
        {
            // TODO: Implement processed Damageable on all the attack
            processedDamagable = new HashSet<IDamageCalculatable>();
        }

        public virtual void PerformAttack()
        {
            if (setPerformAttack)
                enemy.animator.SetTrigger(performAttackTrigger);
        }

        public virtual void DoFrameUpdateLogic() { }
        public virtual void DoPhysicsUpdateLogic() { }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        protected GameObject enemyGameObject;

        [Header("Base Damage Attribute")]
        [SerializeField] float baseDamageAmount = 5.0f;

        public event Action OnAttackEnds;

        public virtual void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            this.enemy = enemyGameObject.GetComponent<EnemyBase>();
            this.enemyGameObject = enemyGameObject;
        }

        public DamageInfo Damage(IDamageCalculatable damageable)
        {
            DamageInfo info = new DamageInfo();
            if (damageable == null)
            {
                return info;
            }

            info.amount = enemy.GetComponent<DamageDealerCalculationPipeline>().GetFinalDealthDamageAmount();
            info.dealer = enemy.gameObject;

            Debug.Log("Damaging Amount: " + info.amount);
            enemy.BaseAtk.Value += 20;

            damageable.Damage(info);
            return info;
        }

        protected void EmitAttackEndsEvent()
        {
            OnAttackEnds?.Invoke();
        }

        public abstract void PerformAttack();
        public virtual void DoFrameUpdateLogic() { }
        public virtual void DoPhysicsUpdateLogic() { }
    }
}

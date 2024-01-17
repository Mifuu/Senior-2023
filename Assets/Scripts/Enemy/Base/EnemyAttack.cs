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
        protected GameObject targetPlayer;
        protected EnemyBase enemy;
        protected GameObject enemyGameObject;

        [Header("Base Damage Attribute")]
        [SerializeField] float baseDamageAmount = 5.0f;

        public event Action OnAttackEnds;

        public virtual void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            this.targetPlayer = targetPlayer;
            this.enemy = enemyGameObject.GetComponent<EnemyBase>();
            this.enemyGameObject = enemyGameObject;
        }

        public DamageInfo Damage(IDamageCalculatable damageable)
        {
            DamageInfo info = new DamageInfo();
            if (damageable == null)
            {
                Debug.Log("Cannot deal damageable to Nondamagable");
                return info;
            }

            info.dealer = enemy.gameObject;
            info.amount = baseDamageAmount;
            damageable.Damage(info);

            Debug.Log("DAMAGING: Dealing " + info.amount + " DMG");
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

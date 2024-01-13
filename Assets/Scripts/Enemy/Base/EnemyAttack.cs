using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public virtual void Initialize(GameObject targetPlayer, GameObject enemyGameObject)
        {
            this.targetPlayer = targetPlayer;
            this.enemy = enemyGameObject.GetComponent<EnemyBase>();
            this.enemyGameObject = enemyGameObject;
        }

        public abstract void PerformAttack();
    }
}

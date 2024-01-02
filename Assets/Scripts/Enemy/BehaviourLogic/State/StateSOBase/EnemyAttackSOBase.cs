using UnityEngine;
using System.Collections.Generic;

namespace Enemy
{
    public class EnemyAttackSOBase : ScriptableObject
    {
        protected EnemyBase enemy;
        protected Transform transform;
        protected GameObject gameObject;

        protected Transform playerTransform;
        [SerializeField] protected List<EnemyAttack> allAttack = new List<EnemyAttack>();

        public void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            this.enemy = enemy;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            var player = GameObject.FindGameObjectWithTag("Player");
            this.playerTransform = player.transform;
            this.allAttack.ForEach((attack) => attack.Initialize(player, gameObject));
        }

        public virtual void DoEnterLogic() { }
        public virtual void DoExitLogic() { ResetValue(); }
        public virtual void DoFrameUpdateLogic() { }
        public virtual void DoPhysicsUpdateLogic() { }
        public virtual void DoAnimationTriggerEventLogic() { }
        public virtual void ResetValue() { }
    }
}

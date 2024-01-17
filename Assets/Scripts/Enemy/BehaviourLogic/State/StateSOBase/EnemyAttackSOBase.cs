using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Enemy
{
    public class EnemyAttackSOBase : ScriptableObject
    {
        protected EnemyBase enemy;
        protected Transform transform;
        protected GameObject gameObject;
        protected Transform playerTransform;
        [SerializeField] protected List<EnemyAttack> allAttack = new List<EnemyAttack>();
        protected EnemyAttack selectedNextAttack;

        public virtual void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            this.enemy = enemy;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            var player = GameObject.FindGameObjectWithTag("Player");
            this.playerTransform = player.transform;

            this.allAttack = this.allAttack.Select((attack) =>
            {
                attack = Instantiate(attack);
                attack.Initialize(player, gameObject);
                return attack;
            }).ToList();

            this.selectedNextAttack = allAttack[0]; // Setting default value
        }

        public virtual void DoEnterLogic() { }
        public virtual void DoExitLogic() { ResetValue(); }
        public virtual void DoFrameUpdateLogic() { selectedNextAttack.DoFrameUpdateLogic(); }
        public virtual void DoPhysicsUpdateLogic() { selectedNextAttack.DoPhysicsUpdateLogic(); }
        public virtual void DoAnimationTriggerEventLogic() { }
        public virtual void ResetValue() { }
    }
}

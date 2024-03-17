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
        [SerializeField] public List<EnemyAttack> allAttack = new List<EnemyAttack>();
        protected EnemyAttack selectedNextAttack;

        public virtual void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            this.enemy = enemy;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;

            this.allAttack = this.allAttack.Select((attack) =>
            {
                attack = Instantiate(attack);
                attack.Initialize(enemy.targetPlayer, gameObject);
                return attack;
            }).ToList();

            this.selectedNextAttack = allAttack[0]; // Setting default value
        }

        public void InitializeWithoutAttack(GameObject gameObject, EnemyBase enemy)
        {
            this.enemy = enemy;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            this.selectedNextAttack = allAttack[0];
        }

        public void RawInitializeAttack(List<EnemyAttack> attack, GameObject gameObject, EnemyBase enemybase, bool requireIns, bool requireInit)
        {
            this.allAttack = this.allAttack.Select((attack) =>
            {
                attack = requireIns ? Instantiate(attack) : attack;
                if (requireInit)
                {
                    attack.Initialize(enemy.targetPlayer, gameObject);
                }
                return attack;
            }).ToList();
        }

        public virtual void DoEnterLogic() { }
        public virtual void DoExitLogic() { ResetValue(); }
        public virtual void DoFrameUpdateLogic() { selectedNextAttack.DoFrameUpdateLogic(); }
        public virtual void DoPhysicsUpdateLogic() { selectedNextAttack.DoPhysicsUpdateLogic(); }
        public virtual void DoAnimationTriggerEventLogic() { }
        public virtual void ResetValue() { }
    }
}

using UnityEngine;

namespace Enemy
{
    public class EnemyReturnSOBase : ScriptableObject
    {
        protected EnemyBase enemy;
        protected Transform transform;
        protected GameObject gameObject;
        
        [Header("Animation")]
        [SerializeField] protected bool setEnterTrigger;
        [SerializeField] protected string enterTriggerName;
        [SerializeField] protected bool setExitTrigger;
        [SerializeField] protected string exitTriggerName;
        [SerializeField] protected string chaseTriggerName;
        protected int enterTrigger;
        protected int exitTrigger;
        protected int chaseTrigger;

        public virtual void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            this.enemy = enemy;
            this.gameObject = gameObject;
            this.transform = gameObject.transform;
            enterTrigger = Animator.StringToHash(enterTriggerName);
            exitTrigger = Animator.StringToHash(exitTriggerName);
            chaseTrigger = Animator.StringToHash(chaseTriggerName);
        }

        public virtual void DoEnterLogic() 
        { 
            if (setEnterTrigger)
                enemy.animator.SetTrigger(enterTrigger);
        }
        public virtual void DoExitLogic() 
        { 
            if (setExitTrigger)
                enemy.animator.SetTrigger(exitTrigger);
            ResetValue(); 
        }
        public virtual void DoFrameUpdateLogic() { }
        public virtual void DoPhysicsUpdateLogic() { }
        public virtual void DoAnimationTriggerEventLogic() { }
        public virtual void ResetValue() { }
    }
}

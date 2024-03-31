using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "TossakanKnockback", menuName = "Enemy/Enemy State/Knockback State/Tossakan Knockback")]
    public class TossakanKnockbackState : EnemyKnockbackSOBase
    {
        private TossakanBossController tossakan;

        private readonly int knockbackTrigger = Animator.StringToHash("Knockback");
        private readonly int phaseChangeTrigger = Animator.StringToHash("PhaseChange");

        public override void Initialize(GameObject gameObject, EnemyBase enemy)
        {
            base.Initialize(gameObject, enemy);
            if (!(enemy is TossakanBossController))
            {
                Debug.LogError("Only Tossakan is allowed to use this state");
                return;
            }

            tossakan = enemy as TossakanBossController;
        }

        public override void DoEnterLogic()
        {
            base.DoEnterLogic();
            if (tossakan.isChangingPhase.Value)
                tossakan.tossakanPuppet.animator.SetTrigger(phaseChangeTrigger);
            else
            {
                tossakan.tossakanPuppet.animator.SetTrigger(knockbackTrigger);
                tossakan.tossakanPuppet.animationEventEmitter.OnKnockbackAnimationEnds += ExitKnockbackState;
            }
        }

        private void ExitKnockbackState()
        {
            tossakan.tossakanPuppet.animationEventEmitter.OnKnockbackAnimationEnds -= ExitKnockbackState;
            tossakan.StateMachine.ChangeState(tossakan.IdleState);
        }
    }
}

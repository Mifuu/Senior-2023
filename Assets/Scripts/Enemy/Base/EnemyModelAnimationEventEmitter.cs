using Unity.Netcode;
using System;

public class EnemyModelAnimationEventEmitter : NetworkBehaviour
{
    public event Action OnAttackAnimationEnds;
    public event Action OnIdleAnimationEnds;
    public event Action OnKnockbackAnimationEnds;
    public event Action OnChaseAnimationEnds;
    public event Action OnPhaseChangeAnimationEnds;
    public event Action OnDieAnimationEnds;

    public void TriggerAttackAnimationEnds() => OnAttackAnimationEnds?.Invoke();
    public void TriggerIdleAnimationEnds() => OnIdleAnimationEnds?.Invoke();
    public void TriggerKnockbackAnimationEnds() => OnKnockbackAnimationEnds?.Invoke();
    public void TriggerChaseAnimationEnds() => OnChaseAnimationEnds?.Invoke();
    public void TriggerPhaseChangeAnimationEnds() => OnPhaseChangeAnimationEnds?.Invoke();
    public void TriggerDieAnimationEnds() => OnDieAnimationEnds?.Invoke();
}

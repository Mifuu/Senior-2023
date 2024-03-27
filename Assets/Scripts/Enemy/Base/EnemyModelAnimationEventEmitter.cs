using Unity.Netcode;
using System;
using UnityEngine;

public class EnemyModelAnimationEventEmitter : NetworkBehaviour
{
    public event Action OnAttackAnimationEnds;
    public event Action OnIdleAnimationEnds;
    public event Action OnKnockbackAnimationEnds;
    public event Action OnChaseAnimationEnds;

    public void TriggerAttackAnimationEnds()
    {
        OnAttackAnimationEnds?.Invoke();
    }

    public void TriggerIdleAnimationEnds()
    {
        OnIdleAnimationEnds?.Invoke();
    }

    public void TriggerKnockbackAnimationEnds()
    {
        OnKnockbackAnimationEnds?.Invoke();
    }

    public void TriggerChaseAnimationEnds()
    {
        Debug.LogWarning("Chase Animation ends");
        OnChaseAnimationEnds?.Invoke();
    }
}

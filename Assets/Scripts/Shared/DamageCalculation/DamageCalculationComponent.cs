using UnityEngine;
using System;
using Unity.Netcode;

public class DamageCalculationComponent : NetworkBehaviour
{
    [SerializeField] public DamagePipeline DealerPipeline = new DamagePipeline();
    [SerializeField] public DamagePipeline ReceiverPipeline = new DamagePipeline();
    public DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer;

    public event Action OnDealerDamageRecalculate;
    public event Action OnReceiverDamageRecalculate;

    public void Awake()
    {
        this.subscriptionContainer = new DamageSubscriptionContainer<float, string, int, bool>(this);
    }

    public void Start()
    {
        DealerPipeline.InitializePipeline(this, gameObject, true);
        ReceiverPipeline.InitializePipeline(this, gameObject, false);
    }

    public override void OnDestroy()
    {
        DealerPipeline.Dispose();
        ReceiverPipeline.Dispose();
    }

    public DamageInfo GetFinalDealthDamageInfo(DamageInfo info = new DamageInfo()) => DealerPipeline.GetValueInfo(info);
    public float GetFinalDealthDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalDealthDamageInfo(info).amount;
    public DamageInfo GetFinalReceivedDamageInfo(DamageInfo info = new DamageInfo()) => ReceiverPipeline.GetValueInfo(info);
    public float GetFinalReceivedDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalReceivedDamageInfo(info).amount;
}

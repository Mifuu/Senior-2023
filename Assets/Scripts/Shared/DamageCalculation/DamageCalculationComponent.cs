using UnityEngine;
using System;
using Unity.Netcode;
using System.Collections.Generic;

public class DamageCalculationComponent : NetworkBehaviour
{
    [SerializeField] public DamagePipeline DealerPipeline = new DamagePipeline();
    [SerializeField] public DamagePipeline ReceiverPipeline = new DamagePipeline();
    public DamageSubscriptionContainer<float, string, int, bool> subscriptionContainer;

    public event Action OnDealerDamageRecalculate;
    public event Action OnReceiverDamageRecalculate;

    private List<Action> ListOfUnSubscriber = new List<Action>();

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
        base.OnDestroy();
        for (int i = 0; i < ListOfUnSubscriber.Count; i++)
        {
            ListOfUnSubscriber[i]?.Invoke();
        }

        ListOfUnSubscriber.RemoveAll((action) => true);

        DealerPipeline.Dispose();
        ReceiverPipeline.Dispose();
    }

    public void RegisterRecalculateEvent(Action action, bool recalculateDealer, bool recalculateReceiver)
    {
        if (recalculateDealer)
        {
            action += DealerPipeline.CalculateAndCache;
            ListOfUnSubscriber.Add(() =>
            {
                action -= DealerPipeline.CalculateAndCache;
            });
        }
        if (recalculateReceiver)
        {
            action += ReceiverPipeline.CalculateAndCache;
            ListOfUnSubscriber.Add(() =>
            {
                action -= ReceiverPipeline.CalculateAndCache;
            });
        }
    }

    public DamageInfo GetFinalDealthDamageInfo(DamageInfo info = new DamageInfo()) => DealerPipeline.GetValueInfo(info);
    public float GetFinalDealthDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalDealthDamageInfo(info).amount;
    public DamageInfo GetFinalReceivedDamageInfo(DamageInfo info = new DamageInfo()) => ReceiverPipeline.GetValueInfo(info);
    public float GetFinalReceivedDamageAmount(DamageInfo info = new DamageInfo()) => GetFinalReceivedDamageInfo(info).amount;
}

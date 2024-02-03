using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class DamageReceiverCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedFactor { get; protected set; }
    public float DefaultValue { get; set; }
    public List<IDamageCalculationUnitBase> StaticModules { get; set; } = new List<IDamageCalculationUnitBase>();
    public List<IDamageCalculationUnitBase> NonStaticModules { get; set; } = new List<IDamageCalculationUnitBase>();

    public void Start()
    {
        foreach(var module in StaticModules) 
        {
            module.Initialize(this, true);
        }

        foreach(var module in NonStaticModules)
        {
            module.Initialize(this, false);
        }
    }

    public void CalculateAndCache()
    {
        CachedFactor = StaticModules.Aggregate(DefaultValue, (aggregatedFactor, next) =>
        {
            if (!next.isEnabled) return aggregatedFactor;
            return next.Calculate(aggregatedFactor);
        });
    }

    public float DamageFactor
    {
        get
        {
            return NonStaticModules.Aggregate(CachedFactor, (aggreatedFactor, next) =>
            {
                if (!next.isEnabled) return aggreatedFactor;
                return next.Calculate(aggreatedFactor);
            });
        }
    }

    public virtual float GetDamageAmount(float initialDamage) => initialDamage * DamageFactor;
}

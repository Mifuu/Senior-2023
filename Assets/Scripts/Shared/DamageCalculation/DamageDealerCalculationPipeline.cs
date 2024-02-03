using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class DamageDealerCalculationPipeline : NetworkBehaviour, IDamageCalculationPipelineBase
{
    public float CachedDamage { get; protected set; }
    public float DefaultValue { get; set; }
    public List<IDamageCalculationUnitBase> StaticModules { get; set; } = new List<IDamageCalculationUnitBase>();
    public List<IDamageCalculationUnitBase> NonStaticModules { get; set; } = new List<IDamageCalculationUnitBase>();

    public void Start()
    {
       foreach (var module in StaticModules) 
       {
           module.Initialize(this);
       }

       foreach (var module in NonStaticModules)
       {
           module.Initialize(this);
       }
    }

    public void CalculateAndCache()
    {
        CachedDamage = StaticModules.Aggregate(DefaultValue, (aggregatedDamage, next) =>
        {
            if (!next.isEnabled) return aggregatedDamage;
            return next.Calculate(aggregatedDamage);
        });
    }

    public float DamageAmount
    {
        get
        {
            return NonStaticModules.Aggregate(CachedDamage, (aggregatedDamage, next) =>
            {
                if (!next.isEnabled) return aggregatedDamage;
                return next.Calculate(aggregatedDamage);
            });
        }
    }
}

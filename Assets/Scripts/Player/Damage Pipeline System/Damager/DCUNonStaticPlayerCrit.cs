using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Damage/Damage/Player Crit")]
public class DCUNonStaticPlayerCrit : DamageCalculationUnitBase
{
    PlayerStat stat;
    BuffManager buff;

    public override void Dispose(DamageCalculationComponent component, SubscriptionRemover remover) { }

    public override void Initialize(DamageCalculationComponent component, SubscriptionAdder adder, bool updateOnChange)
    {
        if (!(component.TryGetComponent<PlayerStat>(out stat) && component.TryGetComponent<BuffManager>(out buff)))
            Debug.LogError("Stat is not found");
    }

    public override DamageInfo CalculateActual(DamageCalculationComponent component, SubscriptionGetter getter, DamageInfo info)
    {
        if (Random.Range(0f, 1f) >= (stat.CritRate * buff.CritBuffTotal))
        {
            info.amount *= stat.CritDMG;
            info.isCrit = true;
        }
        return info;
    }
}

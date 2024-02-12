using System;

public interface IDamageCalculationPipelineBase 
{
    public float DefaultValue { get; set; }
    public void CalculateAndCache();
    public DamageCalculationUnitBase AddUnit(DamageCalculationUnitBase unitBase, bool isStatic);
    public event Action<float, float> OnCacheCalculate;
}

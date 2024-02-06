public interface IDamageCalculationPipelineBase 
{
    public float DefaultValue { get; set; }
    public void CalculateAndCache();
    public DamageCalculationUnitBase AddUnit(DamageCalculationUnitBase unitBase, bool isStatic);
}

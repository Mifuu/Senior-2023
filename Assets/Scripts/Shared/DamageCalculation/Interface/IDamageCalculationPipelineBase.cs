public interface IDamageCalculationPipelineBase 
{
    public float DefaultValue { get; set; }
    public void CalculateAndCache();
    public IDamageCalculationUnitBase AddUnit(IDamageCalculationUnitBase unitBase, bool isStatic);
}

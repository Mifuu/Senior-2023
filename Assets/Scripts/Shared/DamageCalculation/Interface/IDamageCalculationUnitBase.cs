public interface IDamageCalculationUnitBase 
{
    bool isEnabled { get; set; } 
    float PreCalculate(float initialValue); 
    DamageInfo Calculate(DamageInfo info);
    void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange);
}

public interface IDamageCalculationUnitBase 
{
    bool isEnabled { get; set; } 
    float Calculate(float initialValue);
    void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange);
}

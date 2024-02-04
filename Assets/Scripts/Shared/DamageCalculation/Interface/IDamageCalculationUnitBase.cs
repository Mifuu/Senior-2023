using UnityEngine;

public abstract class IDamageCalculationUnitBase : ScriptableObject
{
    public abstract bool IsEnabled { get; set; } 
    public abstract void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange, GameObject owner);
    public virtual float PreCalculate(float initialValue) => initialValue; 
    public virtual DamageInfo Calculate(DamageInfo info) => info; 
}

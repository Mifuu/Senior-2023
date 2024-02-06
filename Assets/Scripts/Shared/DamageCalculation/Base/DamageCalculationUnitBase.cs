using UnityEngine;

public abstract class DamageCalculationUnitBase : ScriptableObject
{
    public abstract bool IsEnabled { get; set; } 
    public abstract void Initialize(IDamageCalculationPipelineBase pipelineBase, bool updateOnChange, GameObject owner);
    public virtual void Dispose() { }
    public virtual float PreCalculate(float initialValue) => initialValue; 
    public virtual DamageInfo Calculate(DamageInfo info) => info; 
    public bool requireInstantiation;
}

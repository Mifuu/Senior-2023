using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageCalculationPipelineBase 
{
    public float DefaultValue { get; set; }
    public void CalculateAndCache();
}

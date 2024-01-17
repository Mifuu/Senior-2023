using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    // Interface Buffable
    // - Contains the extra multiplier for HP, ATK, MOVEMENT SPEED, DEF which defaults to 1 but changed on getting buffed by other character
    // - Must be able to apply buff for some known period of time
    // - IDEA: Contains some IEnumerator function that create a routine
    //
    // TODO: Define IBuffable
    // IEnumerator ApplyBuff(BuffObject)
    // List<BuffObject>
    //
    // Class/struct BuffObject
    // BuffFactor
}

using UnityEngine;
using System;
using System.Collections.Generic;

// <summary>
// Allow the Hitbox or any gameobject with trigger to expose
// its trigger to be check by other component 
// </summary>

#nullable enable
public interface ITriggerCheckable
{
    HashSet<GameObject> ObjectsInTrigger { get; set; }
    HashSet<GameObject> PlayerWithinTrigger { get; set; }
    public event Action<Collider> OnHitboxTriggerEnter;
    public event Action<Collider> OnHitboxTriggerExit;
}

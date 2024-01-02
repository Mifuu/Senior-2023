using UnityEngine;
using System;

public interface ITriggerCheckable
{
    bool isWithinStrikingDistance { get; set; }
    void SetStrikingDistanceBool(bool isWithinStrikingDistance);

    // provides the interface for config to use collisions
    public event Action<Collision> OnEnemyCollide;
    public event Action<Collider> OnEnemyTrigger;
    public void OnTriggerEnter(Collider collider);
    public void OnCollisionEnter(Collision collision);
}

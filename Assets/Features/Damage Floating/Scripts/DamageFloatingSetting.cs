using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageFloatingSetting", menuName = "VFX/DamageFloatingSetting", order = 10)]
public class DamageFloatingSetting : ScriptableObject
{
    [Header("Time Settings")]
    public Vector2 durationRandRange = new Vector2(1f, 1f);
    public AnimationCurve scaleOverLifetime;
    public AnimationCurve alphaOverLifetime;

    [Header("Spawn Settings")]
    public Vector3 randSpawnOffset = new Vector3(0, 0, 0);

    [Header("Move Settings")]
    public float velocityToCamera = 0.1f;
    public float startingUpVelocity = 3;
    public float gravity = 1f;

}

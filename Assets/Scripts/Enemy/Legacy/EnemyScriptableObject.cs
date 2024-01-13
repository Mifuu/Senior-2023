using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

// [CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/Enemy Type")]
[Obsolete("Deprecated: This codes belong to old enemy codebase")]
public class EnemyScriptableObject : ScriptableObject 
{
    public new string name;
    public string description;
    public int hpStat;
    public int atkStat;
    public int defStat;
    public int movementSpdStat;
    public bool isMelee; 
    public bool isFlying;
    public Material mat;
}

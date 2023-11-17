using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/Enemy Type")]
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

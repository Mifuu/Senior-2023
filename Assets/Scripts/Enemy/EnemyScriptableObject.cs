using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/Enemy Type")]
public class EnemyScriptableObject : ScriptableObject 
{
    public new string name;
    public string description;
    public int HP_stat;
    public int ATK_stat;
    public int DEF_stat;
    public int Movement_SPD_stat;
    public Material mat;
}

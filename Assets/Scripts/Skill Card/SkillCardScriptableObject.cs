using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardScriptableObject", menuName = "ScriptableObjects/Skill Card")]
public class SkillCardScriptableObject : ScriptableObject
{
    [SerializeField] float multiplier;
    public float Multiplier {  get { return multiplier; } set {  multiplier = value; } }
}

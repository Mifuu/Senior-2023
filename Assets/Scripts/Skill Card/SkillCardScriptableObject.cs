using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardScriptableObject", menuName = "ScriptableObjects/Skill Card")]
public class SkillCardScriptableObject : ScriptableObject
{
    [SerializeField] 
    float multiplier;
    public float Multiplier {  get { return multiplier; } set {  multiplier = value; } }

    [SerializeField] 
    int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField] 
    GameObject nextLevelPrefab; // prefab of the next level i.e. what the skill card become after it levels up ( hp card lv1 -> hp card lv2 )
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField] 
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description; // The description of this skillCard
    public string Description { get => description; set => description = value; }

    [SerializeField] 
    Sprite icon; // Not mean to be modified in game [Only in Editor]
    public Sprite Icon { get => icon; private set => icon = value; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardScriptableObject", menuName = "ScriptableObjects/Skill Card")]
public class SkillCardScriptableObject : ScriptableObject
{
    [SerializeField] float multiplier;
    public float Multiplier {  get { return multiplier; } set {  multiplier = value; } }

    [SerializeField] int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField] GameObject nextLevelPrefab; // prefab of the next level i.e. what the skill card become after it levels up ( hp card lv1 -> hp card lv2 )
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField] Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement List", menuName = "Achievement/Achievement List")]
public class AchievementList : ScriptableObject
{
    [SerializeField] public string version;
    [SerializeField] public List<BaseAchievement> allAchievement;
}

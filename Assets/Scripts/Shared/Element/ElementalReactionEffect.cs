using UnityEngine;

[CreateAssetMenu(fileName = "ElementalReactionEffect", menuName = "Element/Elemental Reaction")]
public class ElementalReactionEffect : ScriptableObject
{
    public ElementalType primary;
    public ElementalType secondary;
    public string testString;

    public void DoEffect(GameObject applier, GameObject applied)
    {
        Debug.Log(applier + " applied " + testString + " to " + applied);
    }
}

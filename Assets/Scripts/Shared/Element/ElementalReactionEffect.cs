using UnityEngine;

[CreateAssetMenu(fileName = "ElementalReactionEffect", menuName = "Element/Elemental Reaction")]
public class ElementalReactionEffect : ScriptableObject
{
    public ElementalType primary;
    public ElementalType secondary;
    public string testString;

    public void DoEffect(GameObject applier, GameObject applied)
    {
        Debug.LogWarning(applier + " applied " + testString + " to " + applied);
        var enemy = applied.GetComponent<Enemy.EnemyBase>();
        if (enemy != null)
        {
            enemy.StateMachine.ChangeState(enemy.KnockbackState);
        }
    }
}

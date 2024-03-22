using UnityEngine;

[CreateAssetMenu(fileName = "ElementalReactionEffect", menuName = "Element/Elemental Reaction")]
public class ElementalReactionEffect : ScriptableObject
{
    public ElementalType primary;
    public string testString;
    public ElementalType secondary;

    [SerializeField] private GameObject reactionOrbPrefab;

    public void DoEffect(GameObject applier, GameObject applied)
    {
        var networkObject = NetworkObjectPool.Singleton.GetNetworkObject(reactionOrbPrefab, applied.transform.position, applied.transform.rotation);
        networkObject.Spawn();

        var enemy = applied.GetComponent<Enemy.EnemyBase>();
        if (enemy != null)
        {
            enemy.StateMachine.ChangeState(enemy.KnockbackState);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class TossakanArrowSelfAOE : EnemyAOEPlayerSpawnAndDamage
    {
        protected override Vector3 GetSpawnPosition()
        {
            if (NavMesh.SamplePosition(enemy.transform.position, out var hit, 10f, NavMesh.AllAreas))
                return hit.position;
            return enemy.transform.position;
        }
    }
}

using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyLineOfSightCheck : NetworkBehaviour
    {
        private Enemy.EnemyBase _enemy;
        private Transform _enemyTransform;

        public void Awake()
        {
            _enemy = GetComponentInParent<Enemy.EnemyBase>();
            _enemyTransform = _enemy.transform;
        }

        private bool RaycastToPlayersDirection(GameObject targetObject, out RaycastHit hit)
        {
            var direction = (targetObject.transform.position - _enemyTransform.position).normalized;
            return Physics.Raycast(_enemyTransform.position, direction, out hit, float.PositiveInfinity);
        }

        public bool IsTargetPlayerInLineOfSight()
        {
            if (!RaycastToPlayersDirection(_enemy.targetPlayer, out var hit)) return false;
            var health = hit.collider.GetComponentInParent<PlayerHealth>();
            return health != null && health.gameObject == _enemy.targetPlayer;
        }

        public bool IsGameObjectInLineOfSight(GameObject targetObject)
        {
            if (!RaycastToPlayersDirection(targetObject, out var hit)) return false;
            return hit.collider.gameObject == targetObject;
        }
    }
}

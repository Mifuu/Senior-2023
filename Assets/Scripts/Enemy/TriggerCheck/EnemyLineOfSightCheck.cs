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

        // Method has to be called periodically rather than always checking
        public bool IsPlayerInLineOfSight(GameObject targetPLayer) => IsGameObjectInLineOfSight(targetPLayer);
        public bool IsGameObjectInLineOfSight(GameObject targetObject) => Physics.Raycast(_enemyTransform.position, targetObject.transform.position, out RaycastHit hit, float.PositiveInfinity);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Enemy
{
    public class EnemyLineOfSightCheck : NetworkBehaviour
    {
        private GameObject PlayerTarget { get; set; }
        private Enemy.EnemyBase _enemy;
        private Transform _enemyTransform;

        public void Awake()
        {
            // TODO: Properly Init PlayerTarget
            PlayerTarget = GameObject.FindGameObjectWithTag("Player");
            _enemy = GetComponentInParent<Enemy.EnemyBase>();
            _enemyTransform = _enemy.transform;
        }

        // Method has to be called periodically rather than always checking
        public bool IsPlayerInLineOfSight()
        {
            return IsGameObjectInLineOfSight(PlayerTarget);
        }

        public bool IsGameObjectInLineOfSight(GameObject targetObject)
        {
            RaycastHit hit;
            if (Physics.Raycast(_enemyTransform.position, targetObject.transform.position, out hit, float.PositiveInfinity))
            {
                return true;
            }
            return false;
        }
    }
}

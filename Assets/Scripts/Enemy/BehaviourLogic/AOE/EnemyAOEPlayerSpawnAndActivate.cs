using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

namespace Enemy
{
    public abstract class EnemyAOEPlayerSpawnAndActivate : EnemyAOEBase
    {
        [SerializeField] private float timeTillActivate = 3.0f;
        [SerializeField] private float timeTillDestroy = 3.0f;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            transform.position = GetSpawnPosition();
            StartCoroutine(AttackCoroutine());
        }

        protected virtual Vector3 GetSpawnPosition()
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(PlayerTarget.transform.position, out hit, 10f, NavMesh.AllAreas))
                return hit.position;
            return PlayerTarget.transform.position;
        }

        public IEnumerator AttackCoroutine()
        {
            PreEffect();
            yield return new WaitForSeconds(timeTillActivate);
            CancelPreEffect();
            ActivateEffect();
            yield return new WaitForSeconds(timeTillDestroy);
            animator.SetTrigger(endAOEAnimationTrigger);
        }

        public void OnAOEDespawnAnimationEnds()
        {
            DespawnAOE();
            EmitAOEEndsEvent();
        }

        public void DespawnAOE()
        {
            if (!IsServer) return;
            var networkObj = GetComponent<NetworkObject>();
            networkObj.Despawn();
            // NetworkObjectPool.Singleton.ReturnNetworkObject(networkObj, AOEGameObjectPrefab);;
        }
    }
}

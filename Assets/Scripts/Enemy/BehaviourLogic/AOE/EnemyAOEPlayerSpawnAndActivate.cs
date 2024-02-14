using System.Collections;
using UnityEngine;
using Unity.Netcode;

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
            transform.position = PlayerTarget.transform.position;
            StartCoroutine(AttackCoroutine());
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

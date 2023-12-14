using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class EnemyBullet : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
        }
        StartCoroutine(WaitForBulletDespawn());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        NetworkDestroy();
    }

    public IEnumerator WaitForBulletDespawn()
    {
        yield return new WaitForSeconds(5.0f);
        NetworkDestroy();
    }

    public void NetworkDestroy()
    {
        // check for server just in case
        if (!IsServer) return;
        var networkObj = GetComponent<NetworkObject>();
        networkObj.Despawn();
        Destroy(gameObject);
    }
}

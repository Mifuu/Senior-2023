using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TossakanBullet : NetworkBehaviour
{
    Enemy.EnemyBullet bullet;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        if (TryGetComponent<Enemy.EnemyBullet>(out bullet))
        {
            bullet.ChangeHoming(true);
            StartCoroutine(HomingCountdown());
        }
    }

    private IEnumerator HomingCountdown()
    {
        yield return new WaitForSeconds(3.0f);
        if (bullet != null)
            bullet.ChangeHoming(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;

public class Gun : NetworkBehaviour
{
    [FormerlySerializedAs("_bulletPrefab")][SerializeField] GameObject bulletPrefab;
    [SerializeField] private float raycastHitRange = 999f;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform bulletSpawnPosition;

    public void ShootBullet(Camera playerCam, LayerMask aimColliderLayerMask, Transform debugTransform)
    {
        Debug.Log("shoot Bullet");
        if (IsClient && IsOwner)
        {
            Vector3 aimDir;
            Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
            {
                Vector3 mouseWorldPosition = raycastHit.point;
                aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
                Debug.Log("raycast shoot");
            }
            else
            {
                aimDir = (playerCam.transform.forward).normalized;
            }
            Quaternion bulletRotation = Quaternion.LookRotation(aimDir, Vector3.up);
            SpawnBulletServerRpc(NetworkManager.Singleton.LocalClientId, bulletSpawnPosition.position, bulletRotation);
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(ulong localId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = Instantiate(bullet, bulletSpawnPosition, bulletRotation);
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        networkBulletObj.SpawnWithOwnership(localId);
        //StartCoroutine(DespawnBullet(networkBulletObj));
    }
}

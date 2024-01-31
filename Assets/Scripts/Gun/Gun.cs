using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;

public class Gun : NetworkBehaviour
{
    [SerializeField] private float raycastHitRange = 999f;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float shootingDelay = 0.1f;

    private bool canShoot = true;

    public void ShootBullet(Camera playerCam, LayerMask aimColliderLayerMask, Transform debugTransform)
    {
        if (canShoot && IsClient && IsOwner)
        {
            Vector3 aimDir;
            Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
            {
                Vector3 mouseWorldPosition = raycastHit.point;
                aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
                // Debug.Log(raycastHit.collider.name, raycastHit.collider.gameObject);
            }
            else
            {
                aimDir = (playerCam.transform.forward).normalized;
            }
            Quaternion bulletRotation = Quaternion.LookRotation(aimDir, Vector3.up);

            // spawning
            SpawnBulletServerRpc(NetworkManager.Singleton.LocalClientId, bulletSpawnPosition.position, bulletRotation);
            // ProjectileSpawnManager.Singleton.SpawnProjectile(bullet.gameObject, bulletSpawnPosition.position, bulletRotation);

            StartCoroutine(ShootingDelay());
        }
    }

    public void UpdateCanShoot(bool boolean)
    {
        canShoot = boolean;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    private IEnumerator ShootingDelay()
    {
        UpdateCanShoot(false);
        yield return new WaitForSeconds(shootingDelay);
        UpdateCanShoot(true);
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(ulong localId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = Instantiate(bullet, bulletSpawnPosition, bulletRotation);
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        // networkBulletObj.SpawnWithOwnership(localId);
        networkBulletObj.Spawn();
    }
}

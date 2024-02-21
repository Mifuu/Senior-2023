using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;

public class Gun : NetworkBehaviour
{
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform bulletSpawnPosition;
    [SerializeField] private float raycastHitRange = 999f;
    [SerializeField] private float shootingDelay = 0.1f;
    public GunInteractable gunInteractable;

    private bool canShoot = true;
    private bool isOwned = true;

    /*
    private void Start()
    {
        // Load the prefab from the project's assets
        GameObject loadedPrefab = Resources.Load<GameObject>("Path/To/Your/Prefab");

        if (loadedPrefab != null)
        {
            // Instantiate the prefab in the scene
            GameObject instance = Instantiate(loadedPrefab);

            // Get the component from the instantiated prefab
            YourComponentType component = instance.GetComponent<YourComponentType>();

            if (component != null)
            {
                // Component is found, you can now use it
                // For example, you can call a method on the component
                component.YourMethod();
            }
            else
            {
                Debug.LogWarning("Component not found in the prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Prefab not found in the resources.");
        }
    }
    */

    public void ShootBullet(Camera playerCam, LayerMask aimColliderLayerMask)
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
            // SpawnBulletPoolServerRpc(bulletSpawnPosition.position, bulletRotation);

            StartCoroutine(ShootingDelay());
        }
    }

    protected virtual void ShootBullet_(Camera playerCam, LayerMask aimColliderLayerMask)
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
            // SpawnBulletPoolServerRpc(bulletSpawnPosition.position, bulletRotation);

            StartCoroutine(ShootingDelay());
        }
    }

    public void UpdateCanShoot(bool boolean)
    {
        canShoot = boolean;
    }

    public void UpdateIsOwned(bool boolean)
    {
       isOwned = boolean;
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    public bool IsOwned()
    {
        return isOwned;
    }

    private IEnumerator ShootingDelay()
    {
        UpdateCanShoot(false);
        yield return new WaitForSeconds(shootingDelay);
        UpdateCanShoot(true);
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(ulong playerId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = Instantiate(bullet, bulletSpawnPosition, bulletRotation);
        var bulletComponent = bulletObj.GetComponent<BulletProjectile>();
        bulletComponent.PlayerId = playerId; // Pass the player's network object ID
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        // networkBulletObj.SpawnWithOwnership(localId);
        networkBulletObj.Spawn();
    }

    [ServerRpc]
    private void SpawnBulletPoolServerRpc(Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = NetworkObjectPool.Singleton.GetNetworkObject(bullet.gameObject, bulletSpawnPosition, bulletRotation);
        bulletObj.Spawn();
    }
}

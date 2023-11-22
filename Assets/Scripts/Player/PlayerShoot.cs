using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;

public class PlayerShoot : NetworkBehaviour
{
    [FormerlySerializedAs("_bulletPrefab")][SerializeField] GameObject bulletPrefab;
    [SerializeField] private float raycastHitRange = 999f;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform bullet;
    [SerializeField] private Transform bulletSpawnPosition;


    private PlayerInput playerInput;
    private Camera playerCam;
    private InputManager inputManager;

    private void Start()
    {
        playerCam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
    }

    /*
    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * raycastHitRange);
        RaycastHit raycastHit;
        if ( Physics.Raycast(ray, out raycastHit, raycastHitRange, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        
        if (inputManager.shoot)
        {
            Vector3 aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
            Instantiate(bullet, bulletSpawnPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            inputManager.shoot = false;
        }
    }
    */

    
    public void ShootBullet()
    {
        if ( IsClient && IsOwner)
        {
            Vector3 aimDir;
            Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
            {
                //debugTransform.position = raycastHit.point;
                Vector3 mouseWorldPosition = raycastHit.point;
                aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
                Debug.Log("raycas shoot");
               
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

    private static IEnumerator DespawnBullet(NetworkObject bulletObj)
    {
        yield return new WaitForSeconds(8.0f);
        bulletObj.Despawn(true);
    }

    //bulletObj.GetComponent<Rigidbody>().AddForce(transform.forward * 50, ForceMode.VelocityChange);

    //BulletProjectile b = bulletObj.GetComponent<BulletProjectile>();
    //b.bulletDirection = bulletSpawnRotation;
    //b.Init();
}

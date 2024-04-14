using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;
using Unity.Services.Matchmaker.Models;

public class Gun : NetworkBehaviour
{
    [SerializeField] private float raycastHitRange = 999f;
    [SerializeField] private float shootingDelay = 0.1f;
    [SerializeField] public Transform bullet;
    [SerializeField] public Transform bulletSpawnPosition;
    [SerializeField] public GunInteractable gunInteractable;
    [SerializeField] public MuzzleFlash muzzleFlash;
    [SerializeField] public Sprite gunSprite;


    [HideInInspector] public GameObject playerObject;
    [HideInInspector] public ElementalEntity playerEntity;
    [HideInInspector] public ElementAttachable elementAttachable;
    [HideInInspector] public DamageCalculationComponent playerDmgComponent;

    private bool canShoot = true;
    private bool isOwned = true;

    #region Boolean Updater
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
    #endregion

    private void Awake()
    {
        elementAttachable = GetComponent<ElementAttachable>();
        playerObject = PlayerManager.thisClient.gameObject;
        //playerObject = transform.parent.parent.gameObject;
        playerEntity = playerObject.GetComponent<ElementalEntity>();
        playerDmgComponent = playerObject.GetComponent<DamageCalculationComponent>();
    }

    // This is the old code for shooting slow bullet 
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
            SpawnSlowBulletServerRpc(NetworkManager.Singleton.LocalClientId, bulletSpawnPosition.position, bulletRotation);
            // SpawnBulletPoolServerRpc(bulletSpawnPosition.position, bulletRotation);

            StartCoroutine(ShootingDelay());
        }
    }

    // This is the new code for shooting with raycast
    public void ShootBullet_(Camera playerCam, LayerMask aimColliderLayerMask)
    {
        if (canShoot && IsOwner)
        {
            Shoot(playerCam, aimColliderLayerMask, raycastHitRange);
        }
    }

    
    protected virtual void Shoot(Camera playerCam, LayerMask aimColliderLayerMask, float raycastHitRange)
    {
        Vector3 aimDir;
        Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
        {
            Vector3 mouseWorldPosition = raycastHit.point;
            aimDir = (mouseWorldPosition - bulletSpawnPosition.position).normalized;
        }
        else
        {
            aimDir = (playerCam.transform.forward).normalized;
        }
        Quaternion bulletRotation = Quaternion.LookRotation(aimDir, Vector3.up);
        muzzleFlash.PlayVFX();
        SpawnFastBulletServerRpc(NetworkManager.Singleton.LocalClientId, bulletSpawnPosition.position, bulletRotation);
        SFXManager.Instance.PlaySFX("SFX_Pistol_01", gameObject);
        StartCoroutine(ShootingDelay());
    }

    public IEnumerator ShootingDelay()
    {
        UpdateCanShoot(false);
        yield return new WaitForSeconds(shootingDelay);
        UpdateCanShoot(true);
    }

    [ServerRpc]
    public void SpawnFastBulletServerRpc(ulong playerId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        Debug.Log("Spawn fast bullet");
        var bulletObj = Instantiate(bullet, bulletSpawnPosition, bulletRotation);
        var bulletComponent = bulletObj.GetComponent<BulletProjectileEffect>();
        bulletComponent.PlayerId = playerId; // Pass the player's network object ID
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        networkBulletObj.Spawn();
    }

    [ServerRpc]
    private void SpawnSlowBulletServerRpc(ulong playerId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = Instantiate(bullet, bulletSpawnPosition, bulletRotation);

        var bulletComponent = bulletObj.GetComponent<BulletProjectile>();
        bulletComponent.PlayerId = playerId; // Pass the player's network object ID
        bulletComponent.elementalType = elementAttachable.element;
        bulletComponent.entity = playerEntity;
        bulletComponent.BulletInitialize(playerObject);
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        networkBulletObj.Spawn();
    }

    [ServerRpc]
    private void SpawnBulletPoolServerRpc(Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = NetworkObjectPool.Singleton.GetNetworkObject(bullet.gameObject, bulletSpawnPosition, bulletRotation);
        bulletObj.Spawn();
    }
}

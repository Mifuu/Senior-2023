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
    [SerializeField] public Transform normalBullet;
    [SerializeField] public Transform bulletSpawnPosition;
    [SerializeField] public GunInteractable gunInteractable;
    [SerializeField] public MuzzleFlash muzzleFlash;
    [SerializeField] public Sprite gunSprite;
    [SerializeField] private Transform fireBullet;
    [SerializeField] private Transform waterBullet;
    [SerializeField] private Transform earthBullet;
    [SerializeField] private Transform windBullet;

    [HideInInspector] public GameObject playerObject;
    [HideInInspector] public ElementalEntity playerEntity;
    [HideInInspector] public ElementAttachable elementAttachable;
    [HideInInspector] public DamageCalculationComponent playerDmgComponent;

    [HideInInspector] public float shootingSpeedMultiplier = 1f;

    private Transform currentBullet;
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
        //playerObject = PlayerManager.thisClient.gameObject;
        playerObject = transform.parent.parent.gameObject;
        playerEntity = playerObject.GetComponent<ElementalEntity>();
        playerDmgComponent = playerObject.GetComponent<DamageCalculationComponent>();
        OnElementChanged(elementAttachable.element);
        elementAttachable.ElementChanged += OnElementChanged;
    }

    private void OnElementChanged(ElementalType newElement)
    {
        Debug.Log("Element changed to: " + elementAttachable.element);

        switch (elementAttachable.element)
        {
            case ElementalType.Fire:
                currentBullet = fireBullet;
                break;
            case ElementalType.Water:
                currentBullet = waterBullet;
                break;
            case ElementalType.Earth:
                currentBullet = earthBullet;
                break;
            case ElementalType.Wind:
                currentBullet = windBullet;
                break;
            case ElementalType.None:
                currentBullet = normalBullet;
                break;
            default:
                Debug.LogWarning("Unhandled elemental type: " + elementAttachable.element);
                break;
        }
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
        yield return new WaitForSeconds(shootingDelay * shootingSpeedMultiplier);
        UpdateCanShoot(true);
    }

    [ServerRpc]
    public void SpawnFastBulletServerRpc(ulong playerId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        Debug.Log("Spawn fast bullet");
        var bulletObj = Instantiate(currentBullet, bulletSpawnPosition, bulletRotation);
        var bulletComponent = bulletObj.GetComponent<BulletProjectileEffect>();
        bulletComponent.PlayerId = playerId; // Pass the player's network object ID
        var networkBulletObj = bulletObj.GetComponent<NetworkObject>();
        networkBulletObj.Spawn();
    }

    [ServerRpc]
    private void SpawnSlowBulletServerRpc(ulong playerId, Vector3 bulletSpawnPosition, Quaternion bulletRotation)
    {
        var bulletObj = Instantiate(currentBullet, bulletSpawnPosition, bulletRotation);

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
        var bulletObj = NetworkObjectPool.Singleton.GetNetworkObject(currentBullet.gameObject, bulletSpawnPosition, bulletRotation);
        bulletObj.Spawn();
    }
}

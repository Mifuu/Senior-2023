using System.Collections;
using UnityEngine;
using Unity.Netcode;


public class Shotgun : Gun
{
    public int pelletCount = 8; // Number of pellets (bullets) in one shotgun shot
    public float spreadAngle = 5f; // Spread angle for the shotgun pellets (in degrees)

    protected override void Shoot(Camera playerCam, LayerMask aimColliderLayerMask, float raycastHitRange)
    {
        StartCoroutine(ShootingDelay());
        for (int i = 0; i < pelletCount; i++)
        {
            Debug.Log("Shotgun bullet " + i);
            Vector3 shotDirection = CalculateShotDirection(playerCam.transform.forward, spreadAngle);
            Ray ray = new Ray(playerCam.transform.position, shotDirection);

            if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
            {
                Debug.Log("Spawn bullet");
                Vector3 aimDir = (raycastHit.point - bulletSpawnPosition.position).normalized;
                Quaternion bulletRotation = Quaternion.LookRotation(aimDir, Vector3.up);
                SpawnFastBulletServerRpc(NetworkManager.Singleton.LocalClientId, bulletSpawnPosition.position, bulletRotation);

                // Check if the hit object has IDamageCalculatable component
                if (raycastHit.collider.gameObject.TryGetComponent(out IDamageCalculatable damageable))
                {
                    // Create a DamageInfo instance and apply damage to the hit object
                    DamageInfo damageInfo = new DamageInfo(playerObject)
                    {
                        elementalDamageParameter = new ElementalDamageParameter(elementAttachable.element, playerEntity)
                    };
                    damageInfo = playerDmgComponent.GetFinalDealthDamageInfo(damageInfo);
                    damageable.Damage(damageInfo);
                    Debug.Log("Damage dealt to " + raycastHit.collider.gameObject.name + ": " + damageInfo.amount);
                }
            }

            // Play muzzle flash and sound effect
            muzzleFlash.PlayVFX();
            SFXManager.Instance.PlaySFX("SFX_Pistol_01", gameObject); 
        }
    }

    // Calculate a random shot direction within the specified angle range
    private Vector3 CalculateShotDirection(Vector3 baseDirection, float spreadAngle)
    {
        Quaternion randomRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0f);
        return randomRotation * baseDirection;
    }
}

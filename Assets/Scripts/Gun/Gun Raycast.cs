using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;
using System.Data;
using System.ComponentModel;

public class GunRaycast : Gun
{
    protected override void Shoot(Camera playerCam, LayerMask aimColliderLayerMask, float raycastHitRange)
    {
        base.Shoot(playerCam, aimColliderLayerMask, raycastHitRange);
        Ray ray = new(playerCam.transform.position, playerCam.transform.forward);
        // if raycast hit something in its layer
        if (Physics.Raycast(ray, out RaycastHit raycastHit, raycastHitRange, aimColliderLayerMask))
        {
            // if hit object has IDamageCalculatable component 
            if (raycastHit.collider.gameObject.TryGetComponent(out IDamageCalculatable damageable))
            {
                DamageInfo damageInfo = new(playerObject)
                {
                    elementalDamageParameter = new ElementalDamageParameter(elementAttachable.element, playerEntity)
                };
                damageInfo = playerDmgComponent.GetFinalDealthDamageInfo(damageInfo);
                damageable.Damage(damageInfo);
                Debug.Log("Damage dealt to " + raycastHit.collider.gameObject.name + ": " + damageInfo.amount);
            }
        }
    }
}

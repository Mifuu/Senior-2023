using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BulletProjectile : NetworkBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody bulletRigidbody;
    public float speed = 600f;
    public float lifetime = 5.0f;
    public float damageAmount = 1.0f;

    public ulong PlayerId { get; set; }

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bulletRigidbody.velocity = transform.forward * speed;
    }

    public void Update()
    {
        if (!IsServer) return;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            NetworkObject.Despawn(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        IDamageCalculatable damageable = other.GetComponent<IDamageCalculatable>();

        if (damageable != null)
        {
            GameObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(PlayerId).gameObject;
            DamageInfo damageInfo = new DamageInfo(playerObject, damageAmount);
            damageable.Damage(damageInfo); // Pass the player GameObject to the Damage method
            Debug.Log("Bullet Script: " + damageInfo.ToString());
            //Instantiate(vfxHit, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Player Script: No Damagable Class Found");
        }

        // Destroy(gameObject);

        // despawn network object
        NetworkObject.Despawn(true);
        // NetworkObject.Despawn();
    }
}

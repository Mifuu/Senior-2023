using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BulletProjectileEffect : NetworkBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody bulletRigidbody;
    public float speed = 800f;
    public float lifetime = 5.0f;

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
            //SelfDespawnServerRpc();
            NetworkObject.Despawn(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        // Ignore collisions with bullet owner
        PlayerHitboxDamageable playerHitbox = other.GetComponentInChildren<PlayerHitboxDamageable>();
        if (playerHitbox != null && playerHitbox.HasMatchingPlayerId(PlayerId)) return; 

        // Ignore collisions with other bullets from the same shooter
        BulletProjectileEffect otherBullet = other.GetComponent<BulletProjectileEffect>();
        if (otherBullet != null && otherBullet.PlayerId == PlayerId) return;

        Debug.Log("bullet collided with " + other.name);
        //SelfDespawnServerRpc();
        NetworkObject.Despawn(true);
    }

    [ServerRpc]
    private void SelfDespawnServerRpc()
    {
        NetworkObject.Despawn(true);
    }
}

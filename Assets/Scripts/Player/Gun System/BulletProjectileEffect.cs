using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BulletProjectileEffect : NetworkBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody bulletRigidbody;
    public float speed = 1200f;
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
            NetworkObject.Despawn(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject.Despawn(true);
    }
}

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

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        bulletRigidbody.velocity = transform.forward * speed;
    }

    public void Update()
    {
        if (!IsOwner) return;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            // Destroy(gameObject);
            NetworkObject.Despawn(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        IDamageCalculatable damageable = other.GetComponent<IDamageCalculatable>();

        if (damageable != null)
        {
            DamageInfo damageInfo = new DamageInfo(gameObject, damageAmount);
            damageable.Damage(damageInfo);
            //Instantiate(vfxHit, transform.position, Quaternion.identity);
        }
        else
        {
            // Debug.Log("Player Script: No Damagable Class Found");
        }

        // Destroy(gameObject);

        // despawn network object
        NetworkObject.Despawn(true);

    }
}

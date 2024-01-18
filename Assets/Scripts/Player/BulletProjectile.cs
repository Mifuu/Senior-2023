using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BulletProjectile : NetworkBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody bulletRigidbody;
    public float speed = 400f;
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

    private void OnTriggerEnter(Collider other)
    {
        IDamageCalculatable damageable = other.GetComponent<IDamageCalculatable>();

        if (damageable != null)
        {
            DamageInfo damageInfo = new DamageInfo(gameObject, damageAmount);
            damageable.Damage(damageInfo);
            //Instantiate(vfxHit, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

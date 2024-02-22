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
    //public float damageAmount = 1.0f;
    public DamageCalculationComponent component;
    public ElementalType elementalType; 
    public ElementalEntity entity;
    public GameObject playerObject;

    public ulong PlayerId { get; set; }

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        component = GetComponent<DamageCalculationComponent>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bulletRigidbody.velocity = transform.forward * speed;
    }

    public void Initialize(GameObject gameObject)
    {
                
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

    public void bulletInitialize(GameObject player)
    {
        playerObject = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Debug.Log("bullet collide with: " + other.name);
        IDamageCalculatable damageable = other.GetComponentInChildren<IDamageCalculatable>();
        //Debug.Log("damageable: "+damageable.ToString());

        if (damageable == null)
        {
            Debug.LogWarning("Collider does not have IDamageCalculatable component.");
            NetworkObject.Despawn(true);
            return;
        }

        if (component == null) // Warning: Null check is expensive, change later
        {
            component = playerObject.GetComponent<DamageCalculationComponent>();
        }

        DamageInfo damageInfo = new DamageInfo(playerObject);
        damageInfo.elementalDamageParameter = new ElementalDamageParameter(elementalType, entity);
        damageInfo = component.GetFinalDealthDamageInfo(damageInfo);
        damageable.Damage(damageInfo); // Pass the player GameObject to the Damage method
        Debug.Log("Bullet Script: " + playerObject.name);
        //Instantiate(vfxHit, transform.position, Quaternion.identity);

        // Destroy(gameObject);

        NetworkObject.Despawn(true);
      
    }
}

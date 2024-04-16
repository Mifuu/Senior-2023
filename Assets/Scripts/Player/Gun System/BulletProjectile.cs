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

    public void Update()
    {
        if (!IsServer) return;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            NetworkObject.Despawn(true);
        }
    }

    public void BulletInitialize(GameObject player)
    {
        playerObject = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        
        PlayerHitboxDamageable playerHitbox = other.GetComponentInChildren<PlayerHitboxDamageable>(); 
        if (playerHitbox != null && playerHitbox.HasMatchingPlayerId(PlayerId)) return; // return if collide with shooter

        Debug.Log("bullet collide with: " + other.name);
        IDamageCalculatable damageable = other.GetComponentInChildren<IDamageCalculatable>();
        //Debug.Log("damageable: "+damageable.ToString());

        if (damageable == null)
        {
            if(!other.TryGetComponent<IDamageCalculatable>(out damageable))
            {
                Debug.LogWarning("Collider does not have IDamageCalculatable component.");
                NetworkObject.Despawn(true);
                return;
            } 
        }

        if (component == null) // Warning: Null check is expensive, change later
        {
            component = playerObject.GetComponent<DamageCalculationComponent>();
        }

        DamageInfo damageInfo = new(playerObject)
        {
            elementalDamageParameter = new ElementalDamageParameter(elementalType, entity)
        };
        damageInfo = component.GetFinalDealthDamageInfo(damageInfo);
        damageable.Damage(damageInfo); // Pass the player GameObject to the Damage method
        Debug.Log("Damage dealt to " + other.name + ": "  + damageInfo.amount);
        //Instantiate(vfxHit, transform.position, Quaternion.identity);

        // Destroy(gameObject);

        NetworkObject.Despawn(true);
      
    }
}

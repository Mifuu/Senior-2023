using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BulletProjectile : NetworkBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody bulletRigidbody;
    public float speed = 100f;
    public float lifetime = 5.0f; 
    //public Vector3 bulletDirection;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        bulletRigidbody.velocity = transform.forward * speed;
        //DestroyAfterDelayServerRpc();
        //transform.rotation = Quaternion.Euler(bulletDirection); 
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Instantiate(vfxHit, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /*
    [ServerRpc]
    // Coroutine to destroy the bullet after a delay
    private void OnTriggerEnterServerRpc()
    {
        NetworkObject.Destroy(gameObject);
    }
    [ServerRpc]
    private IEnumerator DestroyAfterDelayServerRpc()
    {
        yield return new WaitForSeconds(lifetime);
        NetworkObject.Destroy(gameObject);
    }
    */
}

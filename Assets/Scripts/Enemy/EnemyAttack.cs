using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyAttack : NetworkBehaviour 
{
    [SerializeField] private GameObject bulletPrefab;
    private readonly NetworkVariable<bool> _isShooting = new NetworkVariable<bool>();
    private Enemy _enemy;
    private GameObject _bulletSpawn;

    private void Awake()
    {
        _bulletSpawn = transform.Find("EnemyBulletSpawn").gameObject;
        if (_bulletSpawn == null)
        {
            Debug.LogError("bullet spawn in null");
            enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _enemy = GetComponent<Enemy>();
        if (_enemy == null)
        {
            Debug.LogError("Enemy is null");
            enabled = false;
            return;
        }

        if (!IsServer) return;
        _enemy.isReady.OnValueChanged += (_, current) =>
        {
            _isShooting.Value = true;
            if (!current) return;
            enabled = true;
        };

        _isShooting.OnValueChanged += ((_, current) =>
        {
            if (_enemy.enemyConfig.isMelee || !current) return;
            if (current)
            {
                StartCoroutine(StartShooting());
            }
            else
            {
                StopCoroutine(StartShooting());
            }
        });
    }

    private void Update()
    {
        transform.LookAt(_enemy.followingPlayer.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        _bulletSpawn.transform.LookAt(_enemy.followingPlayer.transform);
    }

    private IEnumerator StartShooting()
    {
        while (true)
        {
            if (IsServer)
            {
                var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                bullet.GetComponent<NetworkObject>().Spawn();
                bullet.GetComponent<Rigidbody>().AddForce(_bulletSpawn.transform.forward * 50.0f, ForceMode.VelocityChange);
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}

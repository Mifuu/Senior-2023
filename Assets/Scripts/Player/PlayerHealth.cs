using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
[Tooltip("This is a player health script")]
public class PlayerHealth : NetworkBehaviour
{
    [Tooltip("Player's health")]
    private NetworkVariable<int> health = new NetworkVariable<int>();
    private CharacterController controller;
    private TextMeshProUGUI healthText;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.detectCollisions = true;
        healthText = GameObject.Find("Canvas/HealthDisplayText").GetComponent<TextMeshProUGUI>();
        healthText.text = "health " + health.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (!other.gameObject.GetComponent<NetworkBehaviour>().IsOwner && IsClient && IsOwner)
            {
                PlayerHitServerRpc();
                StartCoroutine(ChangeHealthTextColor());
            }
        }
    }

    private IEnumerator ChangeHealthTextColor()
    {
        healthText.color = Color.red;
        yield return new WaitForSeconds(5.0f);
        healthText.color = Color.white;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 3;
        }

        if (IsOwner && IsClient)
        {
            health.OnValueChanged += (prev, current) =>
            {
                healthText.text = "health " + current;
            };
        }
    }

    [ServerRpc]
    private void PlayerHitServerRpc()
    {
        health.Value -= 1;
    }
}
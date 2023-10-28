using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(NetworkObject))]
public class EnemyOverlay : NetworkBehaviour
{
    private NetworkVariable<NetworkString> str = new NetworkVariable<NetworkString>();
    [SerializeField] private TextMeshProUGUI overlayText;

    // hp value changed during initial, used to prevent text color changing on setup
    private bool initialHealthSetup = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            GetComponent<EnemyHealth>().HP_stat.OnValueChanged += (_, current) =>
            {
                str.Value = $"Health: {current}";
            };
        }

        str.OnValueChanged += (_, current) =>
        {
            if (!initialHealthSetup)
            {
                initialHealthSetup = true;
            }
            else 
            {
                StartCoroutine(ChangeTextColor());
            }
            overlayText.text = str.Value;
        };
    }

    private IEnumerator ChangeTextColor()
    {
        overlayText.color = Color.red;
        yield return new WaitForSeconds(3.0f);
        overlayText.color = Color.white;
    }
}

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
    private TextMeshProUGUI overlayText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<EnemyHealth>().HP_stat.OnValueChanged += (_, current) => 
        {
            str.Value = $"Health: {str.Value}";
        };

        overlayText.text = str.Value;
        str.OnValueChanged += (_, current) => 
        {
            StartCoroutine(ChangeTextColor());
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

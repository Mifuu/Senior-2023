using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(NetworkObject))]
public class EnemyOverlay : NetworkBehaviour
{
    private readonly NetworkVariable<NetworkString> _str = new NetworkVariable<NetworkString>();
    [SerializeField] private TextMeshProUGUI overlayText;

    // hp value changed during initial, used to prevent text color changing on setup
    private bool _initialHealthSetup = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            GetComponent<EnemyHealth>().hpStat.OnValueChanged += (_, current) =>
            {
                _str.Value = $"Health: {current}";
            };
        }

        _str.OnValueChanged += (_, current) =>
        {
            if (!_initialHealthSetup)
            {
                _initialHealthSetup = true;
            }
            else 
            {
                StartCoroutine(ChangeTextColor());
            }
            overlayText.text = _str.Value;
        };
    }

    private IEnumerator ChangeTextColor()
    {
        overlayText.color = Color.red;
        yield return new WaitForSeconds(3.0f);
        overlayText.color = Color.white;
    }
}

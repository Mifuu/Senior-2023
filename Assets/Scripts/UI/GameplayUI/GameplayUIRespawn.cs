using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIRespawn : MonoBehaviour
{
    public TMP_Text respawnTimeTMP;

    public void OnDeath(float respawnTime)
    {
        StartCoroutine(OnDeathCR(respawnTime));
    }

    IEnumerator OnDeathCR(float respawnTime)
    {
        while (respawnTime > 0)
        {
            respawnTimeTMP.text = $"Respawn in {Mathf.CeilToInt(respawnTime)}";
            yield return new WaitForSeconds(1);
            respawnTime--;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ObserverPattern;

public class EnemyStat : NetworkBehaviour
{
    public NetworkVariable<int> Level = new NetworkVariable<int>(1);
    public Subject<float> BaseDamage = new Subject<float>(5.0f);
    public Subject<float> DamageReceiveFactor = new Subject<float>(1.0f);
    public Dictionary<ElementalType, Subject<float>> ListOfElementalDamageBonus = new Dictionary<ElementalType, Subject<float>>();

    public void ChangeElementalDamageBonusFactor(ElementalType elementalType, float factor, float time = 0f)
    {
        if (!IsServer) return;

        Subject<float> getFactor;
        if (ListOfElementalDamageBonus.TryGetValue(elementalType, out getFactor))
        {
            if (time < 0.1f)
            {
                StartCoroutine(DamageBonusTimer(elementalType, factor, time));
                return;
            }
        }
    }

    private IEnumerator DamageBonusTimer(ElementalType elementalType, float factor, float time)
    {
        yield return new WaitForSeconds(time);
    }
}

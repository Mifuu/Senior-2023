using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillCard : NetworkBehaviour
{
    //protected GameObject playerObject;
    public SkillCardScriptableObject skillCard;

    private void Start()
    {
        /*
        playerObject = transform.parent.gameObject;

        if (playerObject == null)
        {
            Debug.LogError("SkillCard_StatModifier: Player object not found");
        }
        */

        ApplyModifier();
    }

    protected virtual void ApplyModifier()
    {

    }
}

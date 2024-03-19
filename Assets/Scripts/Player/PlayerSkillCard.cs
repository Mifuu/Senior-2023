using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkillCard : NetworkBehaviour
{
    PlayerInventory inventory;
    public int skillCardIndex;
    private GameObject SkillCard;

    public GameObject firstSkillCardTest;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        SpawnSkillCard(firstSkillCardTest);
    }

    
    public void SpawnSkillCard (GameObject skillCard)
    {
        if (!IsOwner) return;

        if (skillCardIndex >= inventory.skillCardSlots.Count - 1)
        {
            Debug.LogError("Skill card slots already full");
            return;
        }
        SkillCard = skillCard;
        SpawnSkillCardServerRPC(transform.position, Quaternion.identity);
        /*
        GameObject spawnedSkillCard = Instantiate(skillCard, transform.position, Quaternion.identity);
        spawnedSkillCard.transform.SetParent(transform);
        inventory.AddSkillCard(skillCardIndex, spawnedSkillCard.GetComponent<SkillCard>());
        skillCardIndex++;
        */
    }

    [ServerRpc]
    private void SpawnSkillCardServerRPC(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var skillCardObject = Instantiate(SkillCard, spawnPosition, spawnRotation);

        var skillCardNetworkObj = skillCardObject.GetComponent<NetworkObject>();
        skillCardNetworkObj.Spawn();
        skillCardNetworkObj.transform.SetParent(transform);
        inventory.AddSkillCard(skillCardIndex, skillCardNetworkObj.GetComponent<SkillCard>());
        skillCardIndex++;
    }
}

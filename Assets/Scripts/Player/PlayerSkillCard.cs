using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static PlayerInventory;

public class PlayerSkillCard : NetworkBehaviour
{
    PlayerInventory inventory;
    PlayerLevel level;
    public int skillCardIndex;
    private GameObject SkillCard;

    //public GameObject firstSkillCardTest;
    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        level = GetComponent<PlayerLevel>();
    }

    
    public void SpawnSkillCard (GameObject skillCard, SkillCardUpgrade chosenSkillCard)
    {
        if (!IsOwner) return;

        if (skillCardIndex >= inventory.skillCardSlots.Count)
        {
            Debug.LogError("Skill card slots already full");
            return;
        }
        SkillCard = skillCard;
        int indexInSkillCardUpgradeOptions = 0;
        for (int i = 0; i < inventory.skillcardUpgradeOptions.Count; i++)
        {
            if (inventory.skillcardUpgradeOptions[i].skillCardData.name == chosenSkillCard.skillCardData.name)
            {
                indexInSkillCardUpgradeOptions = i;
            }
        }
        SpawnSkillCardServerRPC(transform.position, Quaternion.identity, indexInSkillCardUpgradeOptions);
        level.levelSystem.AddSkillCardPoint(-1);

        /*
        GameObject spawnedSkillCard = Instantiate(skillCard, transform.position, Quaternion.identity);
        spawnedSkillCard.transform.SetParent(transform);
        inventory.AddSkillCard(skillCardIndex, spawnedSkillCard.GetComponent<SkillCard>());
        skillCardIndex++;
        */
    }

    [ServerRpc]
    private void SpawnSkillCardServerRPC(Vector3 spawnPosition, Quaternion spawnRotation, int indexInSkillCardUpgradeOptions)
    {
        var skillCardObject = Instantiate(SkillCard, spawnPosition, spawnRotation);

        var skillCardNetworkObj = skillCardObject.GetComponent<NetworkObject>();
        skillCardNetworkObj.Spawn();
        skillCardNetworkObj.transform.SetParent(transform);
        inventory.AddSkillCard(skillCardIndex, skillCardNetworkObj.GetComponent<SkillCard>());
        skillCardIndex++;
        // If card is at full level, remove it from the skillcardUpgradeOptions
        if (skillCardNetworkObj.GetComponent<SkillCard>().skillCardData.NextLevelPrefab.name == "SkillCard_FullLevel")
        {
            Debug.Log($"{inventory.skillcardUpgradeOptions[indexInSkillCardUpgradeOptions].skillCardData.name} has been removed from 3-card list because it's at full level");
            inventory.skillcardUpgradeOptions.RemoveAt(indexInSkillCardUpgradeOptions);
            inventory.RemoveAndApplyUpgrades();
        }
        inventory.RemoveAndApplyUpgrades();
    }
}

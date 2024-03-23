using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using static SkillCard;

public class PlayerInventory : NetworkBehaviour
{
    #region collectible items
    public NetworkVariable<int> Key { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> WaterShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> FireShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> EarthShard { get; set; } = new NetworkVariable<int>(0);
    public NetworkVariable<int> WindShard { get; set; } = new NetworkVariable<int>(0);
    #endregion

    #region collectible item functions

    [ServerRpc]
    public void AddKeyServerRpc(int i)
    {
        Key.Value += i;
    }

    [ServerRpc]
    public void AddWaterShardServerRpc(int i)
    {
        WaterShard.Value += i;
    }

    [ServerRpc]
    public void AddFireShardServerRpc(int i)
    {
        FireShard.Value += i;
    }

    [ServerRpc]
    public void AddEarthShardServerRpc(int i)
    {
        EarthShard.Value += i;
    }

    [ServerRpc]
    public void AddWindShardServerRpc(int i)
    {
        WindShard.Value += i;
    }

    #endregion

    public List<SkillCard> skillCardSlots = new(8);
    public int[] skillCardLevels = new int[8];
    SkillCard _skillCard;
    //public List<Image> skillCardUISlots = new(8);
    private PlayerSkillCard playerSkillCard;
    private PlayerLevel playerLevel;

    [System.Serializable] 
    public class SkillCardUpgrade
    {
        public int skillCardUpgradeIndex;
        public GameObject initialSkillCard;
        public SkillCardScriptableObject skillCardData;
    }

    public List<SkillCardUpgrade> skillcardUpgradeOptions = new();

    private void Start()
    {
        playerSkillCard = GetComponent<PlayerSkillCard>();
        playerLevel = GetComponent<PlayerLevel>();
        RemoveAndApplyUpgrades();
    }

    #region Skill Card Functions
    public void AddSkillCard(int slotIndex, SkillCard skillCard) // Add a skillCard to a specific slot
    {
        skillCardSlots[slotIndex] = skillCard;
        skillCardLevels[slotIndex] = skillCard.skillCardData.Level;
        //skillCardUISlots[slotIndex].enabled = true; // Enable the image component
        //skillCardUISlots[slotIndex].sprite = skillCard.skillCardData.Icon;
        if (skillCard.skillCardData.Icon != null)
        {
            //Debug.Log($"skillcard UI SLOT = {skillCardUISlots[slotIndex]}");
            //Debug.Log($"skillcard icon = {skillCard.skillCardData.Icon}");

            //skillCardUISlots[slotIndex].sprite = skillCard.skillCardData.Icon;
        }
        else
        {
            // maybe assign a blank image 
            Debug.LogError("skill card icon of '" + skillCard.skillCardData.name + "' does not exist");
        }
    }

    public void LevelUpSkillCard (int slotIndex, int upgradeIndex)
    {
        if (!IsOwner) return;

        if (playerLevel.levelSystem.GetSkillCardPoint() > 0)
        {
            if (skillCardSlots.Count > slotIndex)
            {
                _skillCard = skillCardSlots[slotIndex];
                if (!_skillCard.skillCardData.NextLevelPrefab) // check if the nextlevelprefab of the skillcard exist or not
                {
                    Debug.LogError("No next level for " + _skillCard.name);
                    return;
                }
                UpgradeSkillCardServerRPC(slotIndex, transform.position, Quaternion.identity, upgradeIndex);
                playerLevel.levelSystem.AddSkillCardPoint(-1);
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<SkillCardUpgrade> availableSkillCardUpgrade = new(skillcardUpgradeOptions);

        for (int i = 0; i < SkillCardUI.Instance.cardSlotUIs.Length; i++)
        {
            if(availableSkillCardUpgrade.Count == 0) return;
            
            SkillCardUpgrade chosenSkillCardUpgrade = availableSkillCardUpgrade[UnityEngine.Random.Range(0, availableSkillCardUpgrade.Count)];
            availableSkillCardUpgrade.Remove(chosenSkillCardUpgrade);

            if (chosenSkillCardUpgrade != null) // spawn next level card and destroy the old one
            {
                EnableUpgradeUI(i);

                bool newSkillCard = false;
                for (int index = 0; index < skillCardSlots.Count; index++)
                {
                    if (skillCardSlots[index] != null && skillCardSlots[index].skillCardData == chosenSkillCardUpgrade.skillCardData)
                    {
                        newSkillCard = false;
                        if (!newSkillCard)
                        {
                            if (!chosenSkillCardUpgrade.skillCardData.NextLevelPrefab)
                            {
                                DisableUpgradeUI(i);
                                break;
                            }
                            string name = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.name;
                            string description = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.Description;
                            Sprite sprite = chosenSkillCardUpgrade.skillCardData.Icon;
                            SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => LevelUpSkillCard(index, chosenSkillCardUpgrade.skillCardUpgradeIndex), () => RemoveAndApplyUpgrades());  
                        }
                        break;
                    }
                    else
                    {
                        newSkillCard = true;
                    }
                }

                if (newSkillCard) // Spawn a new skillCard
                { 
                    string name = chosenSkillCardUpgrade.skillCardData.name;
                    string description = chosenSkillCardUpgrade.skillCardData.Description;
                    Sprite sprite = chosenSkillCardUpgrade.skillCardData.Icon;
                    SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => playerSkillCard.SpawnSkillCard(chosenSkillCardUpgrade.initialSkillCard), () => RemoveAndApplyUpgrades());
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        SkillCardUI.Instance.RemoveAllListeners();
        DisableUpgradeUI(0); //upgradeoption
        DisableUpgradeUI(1);
        DisableUpgradeUI(2);
    }

    public void RemoveAndApplyUpgrades()
    {
        if (!IsOwner) return;

        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    // Enable card slot in 3-card UI
    void EnableUpgradeUI(int i)
    {
        if (!IsOwner) return;

        //Debug.Log("skill card ui length = " + SkillCardUI.Instance.cardSlotUIs.Length);
        switch (i)
        {
            case 0:
                SkillCardUI.Instance.cardSlot_1.SetActive(true);
                break;
            case 1:
                SkillCardUI.Instance.cardSlot_2.SetActive(true);
                break;
            case 2:
                SkillCardUI.Instance.cardSlot_1.SetActive(true);
                break;
            default:
                Debug.LogError($"Unhandled card slot UI what enabling the slot: slot {i}");
                break;
        }
    }

    // Disable card slot in 3-card UI
    void DisableUpgradeUI(int i)
    {
        if (!IsOwner) return;

        switch (i)
        {
            case 0:
                SkillCardUI.Instance.cardSlot_1.SetActive(false);
                break;
            case 1:
                SkillCardUI.Instance.cardSlot_2.SetActive(false);
                break;
            case 2:
                SkillCardUI.Instance.cardSlot_1.SetActive(false);
                break;
            default:
                Debug.LogError($"Unhandled card slot UI when disabling the slot: slot {i}");
                break;
        }
    }


    [ServerRpc]
    private void UpgradeSkillCardServerRPC(int slotIndex, Vector3 spawnPosition, Quaternion spawnRotation, int upgradeIndex)
    {
        var upgradedSkillCardObject = Instantiate(_skillCard.skillCardData.NextLevelPrefab, spawnPosition, spawnRotation);
        var upgradedSkillCardNetworkObj = upgradedSkillCardObject.GetComponent<NetworkObject>();
        upgradedSkillCardNetworkObj.Spawn();
        upgradedSkillCardNetworkObj.transform.SetParent(transform);
        AddSkillCard(slotIndex, upgradedSkillCardNetworkObj.GetComponent<SkillCard>());
        _skillCard.NetworkObject.Despawn(true);
        skillCardLevels[slotIndex] = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData.Level;
        skillcardUpgradeOptions[upgradeIndex].skillCardData = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData;
    }
    #endregion
}
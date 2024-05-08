using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
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

    #region skill card variables
    public List<SkillCard> skillCardSlots = new(8);
    public int[] skillCardLevels = new int[8];
    SkillCard _skillCard;
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
    #endregion

    private void Start()
    {
        playerSkillCard = GetComponent<PlayerSkillCard>();
        playerLevel = GetComponent<PlayerLevel>();
        // RemoveAndApplyUpgrades();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            RemoveAndApplyUpgrades();
        }
    }

    #region Skill Card Functions
    public void AddSkillCard(int slotIndex, SkillCard skillCard) // Add a skillCard to a specific slot
    {
        skillCardSlots[slotIndex] = skillCard;
        skillCardLevels[slotIndex] = skillCard.skillCardData.Level;
        if (skillCard.skillCardData.Icon == null)
        {
            Debug.LogError("skill card icon of '" + skillCard.skillCardData.name + "' does not exist");
        }
    }

    public void LevelUpSkillCard(int slotIndex, SkillCardUpgrade chosenSkillCardUpgrade)
    {
        if (!IsOwner) return;
        int indexInSkillCardUpgradeOptions = 0;
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
                for (int i = 0; i < skillcardUpgradeOptions.Count; i++)
                {
                    if (skillcardUpgradeOptions[i].skillCardData.name == chosenSkillCardUpgrade.skillCardData.name)
                    {
                        indexInSkillCardUpgradeOptions = i;
                    }
                }
                UpgradeSkillCardServerRPC(slotIndex, transform.position, Quaternion.identity, indexInSkillCardUpgradeOptions);
                playerLevel.levelSystem.AddSkillCardPoint(-1);
            }
        }
    }

    public void ApplyUpgradeOptions()
    {
        if (!IsOwner) return;

        List<SkillCardUpgrade> availableSkillCardUpgrade = new(skillcardUpgradeOptions);

        for (int i = 0; i < SkillCardUI.Instance.cardSlotUIs.Length; i++)
        {
            if (availableSkillCardUpgrade.Count == 0)
            {
                Debug.Log($"Disable Upgrade UI Slot {i} because there is no more card to upgrade");
                DisableUpgradeUI(i);
            }
            else
            {
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
                                SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => LevelUpSkillCard(index, chosenSkillCardUpgrade));
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
                        SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => playerSkillCard.SpawnSkillCard(chosenSkillCardUpgrade.initialSkillCard, chosenSkillCardUpgrade));
                    }
                }
            }
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        if (!IsOwner) return;
        Debug.Log("removeAndApplyUpgrade is called");
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void RemoveUpgradeOptions()
    {
        SkillCardUI.Instance.RemoveAllListeners();
        DisableUpgradeUI(0);
        DisableUpgradeUI(1);
        DisableUpgradeUI(2);
    }

    // Enable card slot in 3-card UI
    void EnableUpgradeUI(int i)
    {
        if (!IsOwner) return;

        switch (i)
        {
            case 0:
                SkillCardUI.Instance.cardSlot_1.SetActive(true);
                break;
            case 1:
                SkillCardUI.Instance.cardSlot_2.SetActive(true);
                break;
            case 2:
                SkillCardUI.Instance.cardSlot_3.SetActive(true);
                break;
            default:
                Debug.LogError($"Unhandled card slot UI what enabling the slot: slot {i}");
                break;
        }
    }

    // Disable card slot in 3-card UI
    void DisableUpgradeUI(int i)
    {
        switch (i)
        {
            case 0:
                SkillCardUI.Instance.cardSlot_1.SetActive(false);
                break;
            case 1:
                SkillCardUI.Instance.cardSlot_2.SetActive(false);
                break;
            case 2:
                SkillCardUI.Instance.cardSlot_3.SetActive(false);
                break;
            default:
                Debug.LogError($"Unhandled card slot UI when disabling the slot: slot {i}");
                break;
        }
    }


    [ServerRpc]
    private void UpgradeSkillCardServerRPC(int slotIndex, Vector3 spawnPosition, Quaternion spawnRotation, int indexInSkillCardUpgradeOptions)
    {
        var upgradedSkillCardObject = Instantiate(_skillCard.skillCardData.NextLevelPrefab, spawnPosition, spawnRotation);
        var upgradedSkillCardNetworkObj = upgradedSkillCardObject.GetComponent<NetworkObject>();
        upgradedSkillCardNetworkObj.Spawn();
        upgradedSkillCardNetworkObj.transform.SetParent(transform);
        AddSkillCard(slotIndex, upgradedSkillCardNetworkObj.GetComponent<SkillCard>());
        _skillCard.NetworkObject.Despawn(true);
        skillCardLevels[slotIndex] = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData.Level;

        // If card is at full level, remove it from the skillcardUpgradeOptions
        if (upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData.NextLevelPrefab.name == "SkillCard_FullLevel")
        {
            Debug.Log($"{skillcardUpgradeOptions[indexInSkillCardUpgradeOptions].skillCardData.name} has been removed from 3-card list because it's at full level");
            skillcardUpgradeOptions.RemoveAt(indexInSkillCardUpgradeOptions);
            RemoveAndApplyUpgrades();
        }
        //If card is not at full level, replace it's skillCardData in skillcardUpgradeOptions with its next level
        else
        {
            skillcardUpgradeOptions[indexInSkillCardUpgradeOptions].skillCardData = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData;
            RemoveAndApplyUpgrades();
        }
    }
    #endregion
}

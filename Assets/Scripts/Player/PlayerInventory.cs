using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

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
    public List<Image> skillCardUISlots = new List<Image>(8);
    private PlayerSkillCard playerSkillCard;

    [System.Serializable] 
    public class SkillCardUpgrade
    {
        public GameObject initialSkillCard;
        public SkillCardScriptableObject skillCardData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public Text upgradeNameDisplay;
        public Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<SkillCardUpgrade> skillcardUpgradeOptions = new();
    public List<UpgradeUI> upgradeUIOptions = new();

    private void Start()
    {
        playerSkillCard = GetComponent<PlayerSkillCard>();
        RemoveAndApplyUpgrades();
    }

    public void AddSkillCard(int slotIndex, SkillCard skillCard) // Add a skillCard to a specific slot
    {
        skillCardSlots[slotIndex] = skillCard;
        skillCardLevels[slotIndex] = skillCard.skillCardData.Level;
        //skillCardUISlots[slotIndex].enabled = true; // Enable the image component
        //skillCardUISlots[slotIndex].sprite = skillCard.skillCardData.Icon;
        if (skillCard.skillCardData.Icon != null)
        {
            skillCardUISlots[slotIndex].sprite = skillCard.skillCardData.Icon;
        }
        else
        {
            // maybe assign a blank image 
            Debug.LogError("skill card icon of '" + skillCard.skillCardData.name + "' does not exist");
        }
    }

    public void LevelUpSkillCard (int slotIndex)
    {
        if (!IsOwner) return;

        if (skillCardSlots.Count > slotIndex)
        {
            _skillCard = skillCardSlots[slotIndex];
            if (!_skillCard.skillCardData.NextLevelPrefab) // check if the nextlevelprefab of the skillcard exist or not
            {
                Debug.LogError("No next level for " + _skillCard.name);
                return;
            }
            UpgradeSkillCardServerRPC(slotIndex, transform.position, Quaternion.identity);
        }
    }

    void ApplyUpgradeOptions()
    {
        for (int i = 0; i < SkillCardUI.Instance.cardSlotUIs.Length; i++)
        {
            //int upgradeType = UnityEngine.Random.Range(1, 3);
            SkillCardUpgrade chosenSkillCardUpgrade = skillcardUpgradeOptions[UnityEngine.Random.Range(0, skillcardUpgradeOptions.Count)];
            if (chosenSkillCardUpgrade != null)
            {
                bool newSkillCard = false;
                for (int index = 0; index < skillCardSlots.Count; index++)
                {
                    if (skillCardSlots[index] != null && skillCardSlots[index].skillCardData == chosenSkillCardUpgrade.skillCardData)
                    {
                        newSkillCard = false;
                        if (!newSkillCard)
                        {
                            /*
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpSkillCard(index)); // Apply button functionality
                            // Set the description and name to be that of the next level
                            upgradeOption.upgradeDescriptionDisplay.text = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.Description;
                            upgradeOption.upgradeNameDisplay.text = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.Description;
                            */

                            string name = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.name;
                            string description = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.Description;
                            Sprite sprite = chosenSkillCardUpgrade.skillCardData.Icon;
                            SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => LevelUpSkillCard(index));
                            
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
                    /*
                    upgradeOption.upgradeButton.onClick.AddListener(() => playerSkillCard.SpawnSkillCard(chosenSkillCardUpgrade.initialSkillCard)); // Apply button functionality
                    // Aplly initial description and name
                    upgradeOption.upgradeDescriptionDisplay.text = chosenSkillCardUpgrade.skillCardData.Description;
                    upgradeOption.upgradeNameDisplay.text = chosenSkillCardUpgrade.skillCardData.name;
                    */

                    string name = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.name;
                    string description = chosenSkillCardUpgrade.skillCardData.NextLevelPrefab.GetComponent<SkillCard>().skillCardData.Description;
                    Sprite sprite = chosenSkillCardUpgrade.skillCardData.Icon;
                    SkillCardUI.Instance.SetCardSlotUI(i, name, description, sprite, () => playerSkillCard.SpawnSkillCard(chosenSkillCardUpgrade.initialSkillCard));
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        SkillCardUI.Instance.RemoveAllListeners();
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    [ServerRpc]
    private void UpgradeSkillCardServerRPC(int slotIndex, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var upgradedSkillCardObject = Instantiate(_skillCard.skillCardData.NextLevelPrefab, spawnPosition, spawnRotation);
        var upgradedSkillCardNetworkObj = upgradedSkillCardObject.GetComponent<NetworkObject>();
        upgradedSkillCardNetworkObj.Spawn();
        upgradedSkillCardNetworkObj.transform.SetParent(transform);
        AddSkillCard(slotIndex, upgradedSkillCardNetworkObj.GetComponent<SkillCard>());
        _skillCard.NetworkObject.Despawn(true);
        skillCardLevels[slotIndex] = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCardData.Level;
    }
}
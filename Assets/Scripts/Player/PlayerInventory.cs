using System;
using System.Collections.Generic;
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

    public void AddSkillCard(int slotIndex, SkillCard skillCard)
    {
        skillCardSlots[slotIndex] = skillCard;
        skillCardLevels[slotIndex] = skillCard.skillCard.Level;
        if (skillCard.skillCard.Icon != null)
        {
            skillCardUISlots[slotIndex].sprite = skillCard.skillCard.Icon;
        }
        else
        {
            // maybe assign a blank image 
            Debug.LogError("skill card icon does not exist");
        }
    }

    public void LevelUpSkillCard (int slotIndex)
    {
        if (!IsOwner) return;

        if (skillCardSlots.Count > slotIndex)
        {
            _skillCard = skillCardSlots[slotIndex];
            if (!_skillCard.skillCard.NextLevelPrefab) // check if the nextlevelprefab of the skillcard exist or not
            {
                Debug.LogError("No next level for " + _skillCard.name);
                return;
            }
            UpgradeSkillCardServerRPC(slotIndex, transform.position, Quaternion.identity);
        }
    }

    [ServerRpc]
    private void UpgradeSkillCardServerRPC(int slotIndex, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        var upgradedSkillCardObject = Instantiate(_skillCard.skillCard.NextLevelPrefab, spawnPosition, spawnRotation);
        var upgradedSkillCardNetworkObj = upgradedSkillCardObject.GetComponent<NetworkObject>();
        upgradedSkillCardNetworkObj.Spawn();
        upgradedSkillCardNetworkObj.transform.SetParent(transform);
        AddSkillCard(slotIndex, upgradedSkillCardNetworkObj.GetComponent<SkillCard>());
        _skillCard.NetworkObject.Despawn(true);
        skillCardLevels[slotIndex] = upgradedSkillCardNetworkObj.GetComponent<SkillCard>().skillCard.Level;
    }
}
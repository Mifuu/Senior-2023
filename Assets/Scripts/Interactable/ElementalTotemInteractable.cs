using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalTotemInteractable : InteractableItem
{
    public enum TotemType
    {
        WaterTotem,
        FireTotem,
        EarthTotem,
        WindTotem,
    }

    public TotemType totemType;
    [SerializeField] private int requireShard;

    protected override void Interact(GameObject playerObject)
    {
        /*
        PlayerSwitchWeapon playerSwitchWeapon = playerObject.GetComponentInChildren<PlayerSwitchWeapon>();
        GameObject holdingGun = playerSwitchWeapon.GetHoldingGun();
        ElementAttachable element= holdingGun.GetComponent<ElementAttachable>();
        int waterShard = playerInventory.WaterShard.Value;
        */

        Debug.Log("interacted with" + gameObject.name);
        PlayerInventory playerInventory = playerObject.GetComponent<PlayerInventory>();
        ElementAttachable gunElement = playerObject.GetComponentInChildren<PlayerSwitchWeapon>().GetHoldingGun().GetComponent<ElementAttachable>();

        // reduce elemental shard of player by an amount of requireShard then change the elementalType of the player's holding gun to the corresponding element
        switch (totemType)
        {
            case TotemType.WaterTotem:
                if (playerInventory.WaterShard.Value >= requireShard)
                {
                    playerInventory.AddWaterShardServerRpc(-requireShard);
                    gunElement.element = ElementalType.Water;
                }
                break;
            case TotemType.FireTotem:
                if (playerInventory.FireShard.Value >= requireShard)
                {
                    playerInventory.AddFireShardServerRpc(-requireShard); 
                    gunElement.element = ElementalType.Fire;
                }
                break;
            case TotemType.EarthTotem:
                if (playerInventory.EarthShard.Value >= requireShard)
                {
                    playerInventory.AddEarthShardServerRpc(-requireShard);
                    gunElement.element = ElementalType.Earth;
                }
                break;
            case TotemType.WindTotem:
                if (playerInventory.WindShard.Value >= requireShard)
                {
                    playerInventory.AddWindShardServerRpc(-requireShard);
                    gunElement.element = ElementalType.Wind;
                }
                break;
            default:
                Debug.LogError($"Unhandled totem type: {totemType}");
                break;
        }
    }
}

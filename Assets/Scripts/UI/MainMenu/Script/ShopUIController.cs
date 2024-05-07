using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform shopItemsLayoutParent;
    [SerializeField] private Button backButton;

    public void Awake()
    {
        CloudService.EconomyService.Singleton.isServiceReady.OnValueChanged += PopulateShopItems;
        if (CloudService.EconomyService.Singleton.isServiceReady.Value) PopulateShopItems(true, true);
#if !DEDICATED_SERVER
        backButton.onClick.AddListener(() => MainMenuUIController.Singleton.menuState.Value = MainMenuUIController.MainMenuState.Main);
#endif
    }

    public void OnDestroy()
    {
        CloudService.EconomyService.Singleton.isServiceReady.OnValueChanged -= PopulateShopItems;
#if !DEDICATED_SERVER
        backButton.onClick.RemoveAllListeners();
#endif
    }

    public void PopulateShopItems(bool prev, bool current)
    {
        if (!current) return;
        foreach (var shopItem in CloudService.EconomyService.Singleton.purchaseDefinitions)
        {
            var acObj = Instantiate(shopItemPrefab);
            acObj.transform.SetParent(shopItemsLayoutParent);
            acObj.transform.localScale = new Vector3(1, 1, 1);
            acObj.SetDetail(shopItem);
        }
    }
}

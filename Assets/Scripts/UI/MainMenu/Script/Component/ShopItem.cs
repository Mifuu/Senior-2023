using UnityEngine;
using Unity.Services.Economy.Model;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI titleObj;
    [SerializeField] private TextMeshProUGUI descriptionObj;
    [SerializeField] private Button purchaseButton;

    VirtualPurchaseDefinition purchaseDefinition;

    public void Awake()
    {
        purchaseButton.onClick.AddListener(MakePurchase);
    }

    public void OnDestroy()
    {
        purchaseButton.onClick.RemoveAllListeners();
    }

    public void SetDetail(VirtualPurchaseDefinition purchaseDefinition)
    {
        this.purchaseDefinition = purchaseDefinition;
        Rerender();
    }

    public async void Rerender()
    {
        titleObj.text = purchaseDefinition.Name;
        descriptionObj.text = purchaseDefinition.Costs[0].Amount.ToString();
        purchaseButton.interactable = await purchaseDefinition.CanPlayerAffordPurchaseAsync();
    }

#if !DEDICATED_SERVER
    public async void MakePurchase()
    {
        MakeVirtualPurchaseResult result = await Unity.Services.Economy.EconomyService
            .Instance.Purchases.MakeVirtualPurchaseAsync(purchaseDefinition.Id);
        Rerender();
    }
#endif
}

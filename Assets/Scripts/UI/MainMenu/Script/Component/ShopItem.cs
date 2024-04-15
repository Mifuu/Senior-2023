using UnityEngine;
using Unity.Services.Economy.Model;
using TMPro;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI titleObj;
    [SerializeField] private TextMeshProUGUI descriptionObj;
    [SerializeField] private TextMeshProUGUI status;

    VirtualPurchaseDefinition purschaseDefinition;

    public void SetDetail(VirtualPurchaseDefinition purchaseDefinition)
    {
        this.purschaseDefinition = purchaseDefinition;
        Rerender();
    }

    public void Rerender()
    {
        titleObj.text = purschaseDefinition.Name;
        descriptionObj.text = purschaseDefinition.Costs[0].Amount.ToString();
    }
}

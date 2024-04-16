using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GunSlotPanel : MonoBehaviour
{
    [SerializeField] Color defaultSlotColor = new(0.1513439f, 0.1590943f, 0.17f, 0.2784314f); // blue-gray
    [SerializeField] Color selectedSlotColor = new(1f, 1f, 1f, 1f); // white

    [SerializeField] Color defaultNumberColor = new(1f, 1f, 1f, 1f); // white
    [SerializeField] Color selectedNumberColor = new(0.1513439f, 0.1590943f, 0.17f, 1f); // blue-gray

    [SerializeField] private UnityEngine.UI.Image gunSlot_1;
    [SerializeField] private UnityEngine.UI.Image gunSlot_2;
    [SerializeField] private UnityEngine.UI.Image gunSlot_3;
    [SerializeField] private UnityEngine.UI.Image line_1;
    [SerializeField] private UnityEngine.UI.Image line_2;
    [SerializeField] private UnityEngine.UI.Image line_3;
    [SerializeField] private TextMeshProUGUI gunNumberText_1;
    [SerializeField] private TextMeshProUGUI gunNumberText_2;
    [SerializeField] private TextMeshProUGUI gunNumberText_3;

    public void HighlightSelectGun(int selectedWeaponNumber)
    {
        switch (selectedWeaponNumber)
        {
            case 0:
                Debug.Log("selectedWeaponNumber is 0");
                gunSlot_1.color = selectedSlotColor;
                gunSlot_2.color = defaultSlotColor;
                gunSlot_3.color = defaultSlotColor;

                gunNumberText_1.color = selectedNumberColor;
                gunNumberText_2.color = defaultNumberColor;
                gunNumberText_3.color = defaultNumberColor;

                line_1.color = selectedNumberColor;
                line_2.color = defaultSlotColor;
                line_3.color = defaultSlotColor;

                break;
            case 1:
                Debug.Log("selectedWeaponNumber is 1");
                gunSlot_1.color = defaultSlotColor;
                gunSlot_2.color = selectedSlotColor;
                gunSlot_3.color = defaultSlotColor;

                gunNumberText_1.color = defaultNumberColor;
                gunNumberText_2.color = selectedNumberColor;
                gunNumberText_3.color = defaultNumberColor;

                line_1.color = defaultSlotColor;
                line_2.color = selectedNumberColor;
                line_3.color = defaultSlotColor;
                break;
            case 2:
                Debug.Log("selectedWeaponNumber is 2");
                gunSlot_1.color = defaultSlotColor;
                gunSlot_2.color = defaultSlotColor;
                gunSlot_3.color = selectedSlotColor;

                gunNumberText_1.color = defaultNumberColor;
                gunNumberText_2.color = defaultNumberColor;
                gunNumberText_3.color = selectedNumberColor;

                line_1.color = defaultSlotColor;
                line_2.color = defaultSlotColor;
                line_3.color = selectedNumberColor;
                break;
            default:
                Debug.Log("selectedWeaponNumber is not 1, 2, or 3");
                break;
        }
    }
}

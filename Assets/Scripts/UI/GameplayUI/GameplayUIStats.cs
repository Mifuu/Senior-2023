using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIStats : MonoBehaviour
{
    public TMP_Text statsText;

    void OnEnable()
    {
        BuffManager b = PlayerManager.thisClient.gameObject.GetComponent<BuffManager>();
        if (b != null)
        {
            BuffManager.Stats stats = b.GetStats();

            statsText.text = "";
            statsText.text += "Stats\n";
            statsText.text += $"HP: \t\t{stats.hp}/{stats.maxHP}\n";
            statsText.text += $"Def: \t\t{stats.def}\n";
            statsText.text += $"Atk: \t\t{stats.atk}\n";
            statsText.text += $"Cri: \t\t{stats.cri}\n";
            statsText.text += $"Spd: \t\t{stats.spd}\n";
            statsText.text += $"Skill CD: \t{stats.skillCD}\n";
            statsText.text += $"Dash Num: \t{stats.dashNum}";
        }
        else
        {
            statsText.text = "Stats\nBuffManager not found!";
        }
    }

}

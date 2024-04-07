using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIStats : MonoBehaviour
{
    public TMP_Text statsText;

    void OnEnable()
    {
        // update the stats text
        // Stats stats = PlayerManager.thisClient.GetStats();
        Stats stats = new Stats();

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

    public struct Stats
    {
        public float hp;
        public float maxHP;
        public float def;
        public float atk;
        public float cri;
        public float spd;
        public float skillCD;
        public int dashNum;
    }
}

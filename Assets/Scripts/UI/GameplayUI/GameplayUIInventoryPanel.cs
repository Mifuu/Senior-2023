using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameplayUI
{
    public class GameplayUIInventoryPanel : MonoBehaviour
    {
        public GameplayUIManager manager;

        [Header("Requirements")]
        public TextMeshProUGUI keyText;
        public TextMeshProUGUI waterShardText;
        public TextMeshProUGUI fireShardText;
        public TextMeshProUGUI earthShardText;
        public TextMeshProUGUI windShardText;

        public void UpdateKeyText(int keyValue)
        {
            keyText.text = "Key: " + keyValue.ToString();
        }

        public void UpdateWaterShardText(int Value)
        {
            waterShardText.text = "Water Shard: " + Value.ToString();
        }
        public void UpdateFireShardText(int Value)
        {
            fireShardText.text = "Fire Shard: " + Value.ToString();
        }
        public void UpdateEarthShardText(int Value)
        {
            earthShardText.text = "Earth Shard: " + Value.ToString();
        }
        public void UpdateWindShardText(int Value)
        {
            windShardText.text = "Wind Shard: " + Value.ToString();
        }
    }
}

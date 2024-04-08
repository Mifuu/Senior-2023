using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "NamedSFXList", menuName = "Sound/NamedSFXList")]
    public class NamedSFXList : ScriptableObject
    {
        public string _name;
        public SFXSound[] sounds;
    }
}

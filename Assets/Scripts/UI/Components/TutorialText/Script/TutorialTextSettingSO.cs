using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial Setting", menuName = "UI/Tutorial/Tutorial Setting")]
public class TutorialTextSettingSO : ScriptableObject
{
    [Tooltip("How long the text would last in seconds")]
    [SerializeField] public float lifeSpan;
    [TextArea(10, 20)]
    [SerializeField] public string text;
    [SerializeField] public bool usePreferredSize;
}

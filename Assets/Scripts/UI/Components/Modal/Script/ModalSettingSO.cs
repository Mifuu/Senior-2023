using UnityEngine;

[CreateAssetMenu(fileName = "Modal Setting", menuName = "UI/Modal/Modal Setting")]
public class ModalSettingSO : ScriptableObject
{
    [Header("Header Component")]
    [SerializeField] public string header_Text;

    [Header("Content Component")]
    [SerializeField] public bool content_IsVertical;
    [SerializeField] public Sprite content_Image;
    [TextArea(10, 20)]
    [SerializeField] public string content_Text;

    [Header("Footer Component")]
    [SerializeField] public bool footer_RemoveConfirmButton;
    [SerializeField] public bool footer_RemoveCancelButton;
    [SerializeField] public bool footer_RemoveAlternateButton;
    [SerializeField] public string footer_AlternateButtonText;
}

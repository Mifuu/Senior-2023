using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ModalController : MonoBehaviour
{
    public struct ModalSetting
    {
        // Header
        public string header_Text;
        // Content
        public bool content_IsVertical;
        public string content_Text;
        public Sprite content_Image;
        // Footer
        public bool footer_RemoveConfirmButton;
        public bool footer_RemoveCancelButton;
        public bool footer_RemoveAlternateButton;
        public string footer_AlternateButtonText;
    }

    #region Setup

    [Header("Header (DO NOT CHANGE)")]
    [SerializeField] public GameObject header;
    [SerializeField] public TextMeshProUGUI headerText;

    [Header("Content (DO NOT CHANGE)")]
    [SerializeField] public GameObject content;
    [SerializeField] public GameObject verticalContent;
    [SerializeField] public TextMeshProUGUI verticalContentText;
    [SerializeField] public Image verticalImage;
    [SerializeField] public GameObject horizontalContent;
    [SerializeField] public TextMeshProUGUI horizontalContentText;
    [SerializeField] public Image horizontalImage;

    [Header("Footer (DO NOT CHANGE)")]
    [SerializeField] public GameObject footer;
    [SerializeField] public Button confirmButton;
    [SerializeField] public Button cancelButton;
    [SerializeField] public Button alternateButton;

    #endregion

    #region Usage

    public Action OnConfirmButtonPressed;
    public Action OnCancelButtonPressed;
    public Action OnAlternateButtonPressed;

    public void Start()
    {
        confirmButton.onClick.AddListener(InvokeConfirmButtonPress);
        cancelButton.onClick.AddListener(InvokeCancelButtonPress);
        alternateButton.onClick.AddListener(InvokeAlternateButtonPress);
    }

    public void OnDestroy()
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        alternateButton.onClick.RemoveAllListeners();
    }

    public ModalController ShowModal(ModalSetting setting)
    {
        SetHeader(setting.header_Text);
        SetFooter(setting.footer_RemoveConfirmButton, setting.footer_RemoveCancelButton, setting.footer_RemoveAlternateButton, setting.footer_AlternateButtonText);

        if (setting.content_Text == "" && setting.content_Image == null)
            content.SetActive(false);
        else
        {
            content.SetActive(true);
            if (setting.content_IsVertical)
                SetVerticalContent(setting.content_Text, setting.content_Image);
            else
                SetHorizontalContent(setting.content_Text, setting.content_Image);
        }

        gameObject.SetActive(true);
        return this;
    }

    public void HideModal()
    {
        gameObject.SetActive(false);
    }

    private void SetHeader(string header_Text)
    {
        header.SetActive(header_Text != "");
        headerText.text = header_Text;
    }

    private void SetHorizontalContent(string content_Text, Sprite content_Image)
    {
        verticalContent.SetActive(false);
        horizontalContent.SetActive(true);

        horizontalContentText.gameObject.SetActive(content_Text != "");
        horizontalContentText.text = content_Text;

        horizontalImage.gameObject.SetActive(content_Image != null);
        horizontalImage.sprite = content_Image;
    }

    private void SetVerticalContent(string content_Text, Sprite content_Image)
    {
        verticalContent.SetActive(true);
        horizontalContent.SetActive(false);

        verticalContentText.gameObject.SetActive(content_Text != "");
        verticalContentText.text = content_Text;

        verticalImage.gameObject.SetActive(content_Image != null);
        verticalImage.sprite = content_Image;
    }

    private void SetFooter(
        bool footer_RemoveConfirmButton,
        bool footer_RemoveCancelButton,
        bool footer_RemoveAlternateButton,
        string footer_AlternateButtonText
        )
    {
        footer.SetActive(!(footer_RemoveCancelButton && footer_RemoveConfirmButton && footer_RemoveCancelButton));
        confirmButton.gameObject.SetActive(!footer_RemoveConfirmButton);
        cancelButton.gameObject.SetActive(!footer_RemoveCancelButton);
        alternateButton.gameObject.SetActive(!footer_RemoveAlternateButton && footer_AlternateButtonText != "");
    }

    private void InvokeConfirmButtonPress() => OnConfirmButtonPressed?.Invoke();
    private void InvokeCancelButtonPress() => OnCancelButtonPressed?.Invoke();
    private void InvokeAlternateButtonPress() => OnAlternateButtonPressed?.Invoke();

    #endregion
}

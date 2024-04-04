using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
    [Header("Dev Mode")]
    [SerializeField] private bool devMode;
    [SerializeField] private Button[] devModeInclude;
    [SerializeField] private GlobalManager.Loader.Scene selectedScene;

    [Header("Match Making")]
    [SerializeField] private Button findGameButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private GlobalManager.NetworkGameManager networkGameManager;

    [Header("UI")]
    [SerializeField] private ModalController modal;

    public void Start()
    {
        if (!devMode)
        {
            foreach (Button button in devModeInclude)
                button.gameObject.SetActive(false);
        }

        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(ShowQuitGameModal);

        if (networkGameManager == null)
        {
            Debug.LogError("NetworkGameManager is not found");
            return;
        }

        if (findGameButton == null) return;
        findGameButton.interactable = false;
        networkGameManager.isAuthenticated.OnValueChanged += ChangeFindGameButtonStatus;
    }

    public void OnDestroy()
    {
        quitGameButton.onClick.RemoveAllListeners();
        findGameButton.onClick.RemoveAllListeners();
        networkGameManager.isAuthenticated.OnValueChanged -= ChangeFindGameButtonStatus;
    }

    private void ChangeFindGameButtonStatus(bool prev, bool current) => findGameButton.interactable = current;
    private void ShowQuitGameModal()
    {
        ModalController.ModalSetting setting = new ModalController.ModalSetting();

        setting.header_Text = "Quit Game?";
        setting.footer_RemoveAlternateButton = true;
        setting.content_Text = "Are you sure you want to quit the game?";

        modal.ShowModal(setting);
        modal.OnConfirmButtonPressed += QuitGame;
        modal.OnCancelButtonPressed += CloseModal;
    }

    private void CloseModal()
    {
        modal.HideModal();
        modal.OnCancelButtonPressed -= CloseModal;
        modal.OnConfirmButtonPressed -= QuitGame;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }
}

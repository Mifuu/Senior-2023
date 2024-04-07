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
    [SerializeField] private Button achievementGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private GlobalManager.NetworkGameManager networkGameManager;

    [Header("UI Modal")]
    [SerializeField] private ModalController modal;
    [SerializeField] private ModalSettingSO contentNotReadyModalSetting;

    [Header("Test Tutorial Text")]
    [SerializeField] private TutorialTextController tutorial;
    [SerializeField] private TutorialTextSettingSO somethingWicked;

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

        if (findGameButton != null)
            findGameButton.interactable = false;

        if (achievementGameButton != null)
            achievementGameButton.onClick.AddListener(ShowContentNotReadyModal);

        if (settingButton != null)
            settingButton.onClick.AddListener(ShowSomethingWicked);

        networkGameManager.isAuthenticated.OnValueChanged += ChangeFindGameButtonStatus;
    }

    public void OnDestroy()
    {
        quitGameButton.onClick.RemoveAllListeners();
        findGameButton.onClick.RemoveAllListeners();
        achievementGameButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
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
        modal.OnCancelButtonPressed += CloseQuitGameModal;
    }

    private void ShowInitializeModal()
    {
        ModalController.ModalSetting setting = new ModalController.ModalSetting();

        setting.header_Text = "Initializing Services...";
        setting.content_Text = "Please wait while the game services is initializing...";
        setting.footer_RemoveAlternateButton = true;
    }

    private void ShowContentNotReadyModal()
    {
        modal.ShowModal(contentNotReadyModalSetting);
        modal.OnConfirmButtonPressed += CloseContentNotReadyModal;
    }

    private void CloseQuitGameModal()
    {
        modal.HideModal();
        modal.OnCancelButtonPressed -= CloseQuitGameModal;
        modal.OnConfirmButtonPressed -= QuitGame;
    }

    private void CloseContentNotReadyModal()
    {
        modal.HideModal();
        modal.OnConfirmButtonPressed -= CloseContentNotReadyModal;
    }

    private void ShowSomethingWicked() => tutorial.ShowTutorialText(somethingWicked);

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }
}

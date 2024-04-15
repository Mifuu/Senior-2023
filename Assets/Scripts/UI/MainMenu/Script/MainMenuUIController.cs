using UnityEngine;
using UnityEngine.UI;
using System;
using ObserverPattern;

public class MainMenuUIController : MonoBehaviour
{
#if !DEDICATED_SERVER
    public static MainMenuUIController Singleton;

    [Header("Dev Mode")]
    [SerializeField] private bool devMode;
    [SerializeField] private Button[] devModeInclude;
    [SerializeField] private GlobalManager.Loader.Scene selectedScene;

    [Header("Button")]
    [SerializeField] private Button findGameButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button achievementGameButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private GlobalManager.NetworkGameManager networkGameManager;

    [Header("Modal")]
    [SerializeField] private ModalController modal;
    [SerializeField] private ModalSettingSO contentNotReadyModalSetting;
    private Action currentModalCleanup;

    [Header("Panel")]
    [SerializeField] private GameObject authPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private GameObject shopPanel;
    public Subject<MainMenuState> menuState = new Subject<MainMenuState>(MainMenuState.Authentication);

    public enum MainMenuState
    {
        Main,
        Setting,
        Credit,
        Authentication,
        Achievement,
        Shop
    }

    public void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(this);
    }

    public void Start()
    {
        if (!devMode)
        {
            foreach (Button button in devModeInclude)
                button.gameObject.SetActive(false);
        }

        if (networkGameManager == null)
        {
            Debug.LogError("NetworkGameManager is not found");
            return;
        }

        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(ShowQuitGameModal);

        if (findGameButton != null)
        {
            findGameButton.interactable = false;
            findGameButton.onClick.AddListener(CloudService.MatchMakingService.Singleton.BeginFindingMatch);
        }

        if (achievementGameButton != null)
            achievementGameButton.onClick.AddListener(() => menuState.Value = MainMenuState.Achievement);

        if (creditButton != null)
            creditButton.onClick.AddListener(() => menuState.Value = MainMenuState.Credit);

        if (shopButton != null)
            shopButton.onClick.AddListener(() => menuState.Value = MainMenuState.Shop);

        if (settingButton != null)
            settingButton.onClick.AddListener(() => menuState.Value = MainMenuState.Setting);

        CloudService.AuthenticationService.Singleton.isAuthenticated.OnValueChanged += ChangeFindGameButtonStatus;
        menuState.OnValueChanged += ChangeMenu;
        menuState.Value = MainMenuState.Authentication;
    }

    public void OnDestroy()
    {
        quitGameButton.onClick.RemoveAllListeners();
        findGameButton.onClick.RemoveAllListeners();
        achievementGameButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        CloudService.AuthenticationService.Singleton.isAuthenticated.OnValueChanged -= ChangeFindGameButtonStatus;
        menuState.OnValueChanged -= ChangeMenu;
    }

    public void ChangeMenu(MainMenuState prev, MainMenuState current)
    {
        var newMain = false;
        var newSetting = false;
        var newCredit = false;
        var newAuth = false;
        var newAchieve = false;
        var newShop = false;

        switch (current)
        {
            case MainMenuState.Main:
                newMain = true;
                break;
            case MainMenuState.Setting:
                newSetting = true;
                break;
            case MainMenuState.Credit:
                newCredit = true;
                break;
            case MainMenuState.Authentication:
                newAuth = true;
                break;
            case MainMenuState.Achievement:
                newAchieve = true;
                break;
            case MainMenuState.Shop:
                newShop = true;
                break;
        }

        mainMenuPanel?.SetActive(newMain);
        settingPanel?.SetActive(newSetting);
        creditPanel?.SetActive(newCredit);
        authPanel?.SetActive(newAuth);
        achievementPanel?.SetActive(newAchieve);
        shopPanel?.SetActive(newShop);
    }

    private void ChangeFindGameButtonStatus(bool prev, bool current) => findGameButton.interactable = current;

    private void ShowQuitGameModal()
    {
        ModalController.ModalSetting setting = new ModalController.ModalSetting();

        setting.header_Text = "Quit Game?";
        setting.footer_RemoveAlternateButton = true;
        setting.content_Text = "Are you sure you want to quit the game?";

        currentModalCleanup = () =>
        {
            modal.OnCancelButtonPressed -= QuitGame;
            modal.OnCancelButtonPressed -= CloseModal;
        };

        modal.OnConfirmButtonPressed += QuitGame;
        modal.OnCancelButtonPressed += CloseModal;
        modal.ShowModal(setting);
    }

    private void ShowInitializeModal()
    {
        ModalController.ModalSetting setting = new ModalController.ModalSetting();

        setting.header_Text = "Initializing Services...";
        setting.content_Text = "Please wait while the game services is initializing...";
        setting.footer_RemoveAlternateButton = true;
    }

    private void CloseModal()
    {
        modal.HideModal();
        currentModalCleanup?.Invoke();
        currentModalCleanup = null;
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }
#endif
}

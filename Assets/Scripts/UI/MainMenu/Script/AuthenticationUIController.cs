using UnityEngine;
using ObserverPattern;
using UnityEngine.UI;
using TMPro;

public class AuthenticationUIController : MonoBehaviour
{
    private string username = "";
    private string password = "";
    private string confirmPassword = "";
    private Subject<bool> isOnLogin = new Subject<bool>(true);

    public void OnUsernameChanged(string value) => username = value;
    public void OnPasswordChange(string value) => password = value;
    public void OnConfirmPasswordChange(string value) => confirmPassword = value;

    [SerializeField] private Button switchButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject confirmPasswordGroup;
    [SerializeField] private RectTransform rectTransform;

    public void Start()
    {
        SwitchPage(false, true);
        isOnLogin.OnValueChanged += SwitchPage;
        switchButton.onClick.AddListener(OnSwitchButtonPressed);
        confirmButton.onClick.AddListener(OnConfirmButtonPressed);
    }

    public void OnDestroy()
    {
        isOnLogin.OnValueChanged -= SwitchPage;
        switchButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
    }

    public void OnSwitchButtonPressed() => isOnLogin.Value = !isOnLogin.Value;

    public void OnConfirmButtonPressed()
    {
        if (isOnLogin.Value)
            CloudService.AuthenticationService.Singleton.AttempSignIn(username, password);
        else
            CloudService.AuthenticationService.Singleton.AttempSignUp(username, password);
    }

    private void SwitchPage(bool prev, bool current)
    {
        if (current)
        {
            confirmPasswordGroup.SetActive(false);
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sign In";
            switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go to Sign up";
            rectTransform.sizeDelta = new Vector2(1000, 600);
        }
        else
        {
            confirmPasswordGroup.SetActive(true);
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sign Up";
            switchButton.GetComponentInChildren<TextMeshProUGUI>().text = "Go to Sign In";
            rectTransform.sizeDelta = new Vector2(1000, 800);
        }
    }
}

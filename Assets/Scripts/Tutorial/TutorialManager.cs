using UnityEngine;
using Unity.Netcode;
using GlobalManager;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject markerPrefeb;

    public void Start()
    {
        Cursor.visible = false;
        NetworkManager.Singleton.StartHost();
    }

    public void QuitTutorial()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenu);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartGameOnLoad : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Singleton.StartHost();
        MultiplayerGameManager.Instance.StartGame();
    }
}

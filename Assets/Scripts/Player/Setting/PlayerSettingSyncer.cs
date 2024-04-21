using UnityEngine;
using UnityEngine.Audio;

public class PlayerSettingSyncer : MonoBehaviour
{
    PlayerLook playerLook;
    [SerializeField] AudioMixer mixer;

    public void Awake()
    {
        playerLook = GetComponent<PlayerLook>();
    }

    public void OnNetworkSpawn()
    {
        SyncSensitivity();
        SyncVolumn();
    }

    private void SyncSensitivity()
    {
        playerLook.xSensitivity = PlayerSettingManager.sensitivity;
        playerLook.ySensitivity = PlayerSettingManager.sensitivity;
    }

    private void SyncVolumn() => mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerSettingManager.volumn / 100) * 20f);
}

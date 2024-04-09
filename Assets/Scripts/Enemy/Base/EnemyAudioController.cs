using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace Enemy
{
    public class EnemyAudioController : NetworkBehaviour
    {
        public static EnemyAudioController Singleton;

        [SerializeField] public NamedSFXList[] namedSFXList;
        [SerializeField] private AudioSource SFXObject;
        private Dictionary<string, EnemyAudioControllerSingular> sfxListMap = new Dictionary<string, EnemyAudioControllerSingular>();
        private Dictionary<string, Dictionary<string, SFXSound>> sfxSoundMap = new Dictionary<string, Dictionary<string, SFXSound>>();

        public void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            else
                Destroy(this);

            if (namedSFXList == null)
                Debug.LogError("[AWAKE ERROR] EnemyAudioController: NamedSFXList is null, please add one to the SFXManager");

            ConvertToDictionary();
        }

        private void ConvertToDictionary()
        {
            for (int i = 0; i < namedSFXList.Length; i++)
            {
                var current = namedSFXList[i];
                if (sfxListMap.ContainsKey(current._name) || sfxSoundMap.ContainsKey(current._name))
                {
                    Debug.LogWarning($"[SETUP ERROR] EnemyAudioController: Sound list named \"{current._name}\" has a duplicate");
                    continue;
                }

                sfxListMap.Add(current._name, new EnemyAudioControllerSingular(current._name));
                var SFXDict = new Dictionary<string, SFXSound>();
                for (int j = 0; j < current.sounds.Length; j++)
                {
                    var currentSFX = current.sounds[j];
                    if (!SFXDict.TryAdd(currentSFX.name, currentSFX))
                        Debug.LogWarning($"[SETUP ERROR] EnemyAudioController: Sound named \"{currentSFX.name}\" has a duplicate in list named \"{current._name}\"");
                }
                sfxSoundMap.Add(current._name, SFXDict);
            }
        }

        public EnemyAudioControllerSingular Get(string listName) 
        {
            EnemyAudioControllerSingular controllerSingular;
            if (sfxListMap.TryGetValue(listName, out controllerSingular))  
                return controllerSingular;
            else
            {
                Debug.LogError($"[GET ERROR] EnemyAudioController: Cant find list with name \"{listName}\"");
                return null;
            }
        }

        public bool CheckIsSoundAvailable(string listName, string audioName)
        {
            return sfxSoundMap.TryGetValue(listName, out var innerSoundDict) && innerSoundDict.TryGetValue(audioName, out var _);
        }

        public void PlaySFXAtObject(string listName, string audioName, Vector3 spawnPosition, bool localOnly = false)
        {
            if (localOnly)
            {
                _PlaySFXOnObject(listName, audioName, spawnPosition);
                return;
            }

            if (IsClient)
            {
                _PlaySFXOnObject(listName, audioName, spawnPosition);
                PlaySFXOnAllClientClientRpc(listName, audioName, spawnPosition, NetworkManager.Singleton.LocalClientId);
            }
            else
                PlaySFXOnAllClientServerRpc(listName, audioName, spawnPosition);
        }

        private void _PlaySFXOnObject(string listName, string audioName, Vector3 spawnPosition)
        {
            if (!sfxSoundMap.TryGetValue(listName, out var soundMap))
            {
                Debug.LogError($"[PLAY ERROR] EnemyAudioController: Sound List name \"{listName}\" can not be found");
                return;
            }

            if (!soundMap.TryGetValue(audioName, out var audio))
            {
                Debug.LogError($"[PLAY ERROR] EnemyAudioController: Sound \"{audioName}\"not found in list named \"{listName}\"");
                return;
            }

            AudioSource audioSource = Instantiate(SFXObject, spawnPosition, Quaternion.identity);
            if (audio.audioMixerGroup != null) // Null check can be expensive na ja
                audioSource.outputAudioMixerGroup = audio.audioMixerGroup;

            audioSource.clip = audio.clip;
            audioSource.volume = audio.volume;
            audioSource.minDistance = 0f;
            audioSource.maxDistance = 30f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.spatialBlend = 1f;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }

        [ClientRpc]
        private void PlaySFXOnAllClientClientRpc(NetworkString listName, NetworkString soundName, Vector3 position, ulong exceptClient)
        {
            if (exceptClient == NetworkManager.Singleton.LocalClientId) return;
            _PlaySFXOnObject(listName, soundName, position);
        }

        [ServerRpc]
        private void PlaySFXOnAllClientServerRpc(NetworkString listName, NetworkString soundName, Vector3 position)
        {
            PlaySFXOnAllClientClientRpc(listName, soundName, position, NetworkManager.Singleton.LocalClientId);
        }
    }

    public class EnemyAudioControllerSingular
    {
        public EnemyAudioControllerSingular(string name)
        {
            this._name = name;
        }

        public string _name;
        public void PlaySFXAtObject(string name, Vector3 position) => EnemyAudioController.Singleton.PlaySFXAtObject(_name, name, position);
        public bool CheckIsSoundAvailable(string audioName) => EnemyAudioController.Singleton.CheckIsSoundAvailable(_name, audioName);
    }
}

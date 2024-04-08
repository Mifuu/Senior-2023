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
                Debug.LogError("[ERROR] SFXManager: SFXList is null, please add one to the SFXManager");

            ConvertToDictionary();
        }

        private void ConvertToDictionary()
        {
            for (int i = 0; i < namedSFXList.Length; i++)
            {
                var current = namedSFXList[i];
                if (sfxListMap.ContainsKey(current._name) || sfxSoundMap.ContainsKey(current._name))
                {
                    Debug.LogWarning($"Sound list named {current._name} has not been added since there's already a sound list with that name");
                    continue;
                }

                sfxListMap.Add(current._name, new EnemyAudioControllerSingular(current._name));
                var SFXDict = new Dictionary<string, SFXSound>();
                for (int j = 0; j < current.sounds.Length; j++)
                {
                    var currentSFX = current.sounds[j];
                    if (!SFXDict.TryAdd(currentSFX.name, currentSFX))
                        Debug.LogWarning($"Sound named {currentSFX.name} has not been added since it has a duplicate in list named {current._name}");
                }
                sfxSoundMap.Add(current._name, SFXDict);
            }
        }

        public EnemyAudioControllerSingular Get(string listName) => sfxListMap.GetValueOrDefault(listName);

        public void PlaySFXOnObject(string listName, string audioName, Vector3 spawnTransform, bool localOnly = false)
        {
            if (localOnly)
            {
                _PlaySFXOnObject(listName, audioName, spawnTransform);
                return;
            }

            if (IsClient)
            {
                _PlaySFXOnObject(listName, audioName, spawnTransform);
                PlaySFXOnAllClientClientRpc(listName, audioName, spawnTransform, NetworkManager.Singleton.LocalClientId);
            }
            else
                PlaySFXOnAllClientServerRpc(listName, audioName, spawnTransform);
        }

        private void _PlaySFXOnObject(string listName, string audioName, Vector3 spawnTransform)
        {
            if (!sfxSoundMap.TryGetValue(listName, out var soundMap))
            {
                Debug.LogError("Sound Namespace Not found in the list");
                return;
            }

            if (!soundMap.TryGetValue(audioName, out var audio))
            {
                Debug.LogError("Sound Not found in list name " + listName);
                return;
            }

            AudioSource audioSource = Instantiate(SFXObject, spawnTransform, Quaternion.identity);
            if (audio.audioMixerGroup != null) // Null check can be expensive na ja
                audioSource.outputAudioMixerGroup = audio.audioMixerGroup;

            audioSource.clip = audio.clip;
            audioSource.volume = audio.volume;
            audioSource.minDistance = 0f;
            audioSource.maxDistance = 30f;
            audioSource.Play();
            float clipLength = audioSource.clip.length;
            Destroy(audioSource.gameObject, clipLength);
        }

        [ClientRpc]
        private void PlaySFXOnAllClientClientRpc(NetworkString listName, NetworkString soundName, Vector3 transform, ulong exceptClient)
        {
            if (exceptClient == NetworkManager.Singleton.LocalClientId) return;
            _PlaySFXOnObject(listName, soundName, transform);
        }

        [ServerRpc]
        private void PlaySFXOnAllClientServerRpc(NetworkString listName, NetworkString soundName, Vector3 transform)
        {
            PlaySFXOnAllClientClientRpc(listName, soundName, transform, NetworkManager.Singleton.LocalClientId);
        }
    }

    public class EnemyAudioControllerSingular
    {
        public EnemyAudioControllerSingular(string name)
        {
            this._name = name;
        }

        public string _name;
        public void PlaySFXAtObject(string name, Vector3 transform) => EnemyAudioController.Singleton.PlaySFXOnObject(_name, name, transform);
    }
}

using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace GlobalManager
{
    public class Loader 
    {
        public enum Scene
        {
            MainMenu,
            Sprint7Showcase,
            Game,
        }

        public static void Load(Scene scene) => SceneManager.LoadScene(scene.ToString());
        public static void LoadNetwork(Scene scene) => NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);

        public static AsyncOperation _LoadAsync(Scene scene) => SceneManager.LoadSceneAsync(scene.ToString());
        public static IEnumerator LoadAsync(Scene scene)
        {
            var asyncLoad = _LoadAsync(scene);
            while (!asyncLoad.isDone)
                yield return null;
        }

        #region Game Load

        private static Scene DefaultGameScene = Scene.Sprint7Showcase;

        public static void LoadGame() => Load(DefaultGameScene);
        public static AsyncOperation _LoadGameAsync() => _LoadAsync(DefaultGameScene);
        public static IEnumerator LoadGameAsync() => LoadAsync(DefaultGameScene);

        #endregion
    }
}

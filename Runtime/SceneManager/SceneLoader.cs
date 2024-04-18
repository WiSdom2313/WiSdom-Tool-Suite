using System.Collections;
using UnityEngine;
using WiSdom.DesignPattern;

namespace WiSdom.Core
{
    public class SceneLoader : Singleton<SceneLoader>
    {

        protected override void Awake()
        {
            base.Awake();
        }

        // Load scene using traditional SceneManager
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                // Optionally update progress UI here
                yield return null;
            }
        }

        // // Load scene using Addressables
        // public void LoadSceneAddressable(string sceneName)
        // {
        //     StartCoroutine(LoadSceneAddressableAsync(sceneName));
        // }

        // private IEnumerator LoadSceneAddressableAsync(string sceneName)
        // {
        //     AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        //     while (!handle.IsDone)
        //     {
        //         // Optionally update progress UI here
        //         yield return null;
        //     }
        // }

        // Unload scene
        public void UnloadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

        // // Unload scene using Addressables
        // public void UnloadSceneAddressable(string sceneName)
        // {
        //     Addressables.UnloadSceneAsync(Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single));
        // }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public static class AsyncSceneManager
{
   private static AsyncOperation operation;

   public static void LoadScene(string sceneName, bool activation)
   {
      operation = SceneManager.LoadSceneAsync(sceneName);
      operation.allowSceneActivation = activation;
   }

   public static void ActivateScene()
   {
      operation.allowSceneActivation = true;
   }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public static class AsyncSceneManager
{
   public static AsyncOperation operation;

   public static void AsyncLoad(string sceneName, bool activation)
   {
      operation = SceneManager.LoadSceneAsync(sceneName);
      operation.allowSceneActivation = activation;
      
      
   }

   public static void ActiveAfterOperationDone(AsyncOperation operation,GameObject obj)
   {
      
   }

   public static void ActivateScene()
   {
      operation.allowSceneActivation = true;
   }
}

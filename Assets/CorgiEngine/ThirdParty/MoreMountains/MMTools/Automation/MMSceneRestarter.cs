using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools
{
    /// <summary>
    /// This component lets you restart a scene by pressing a key
    /// </summary>
    public class MMSceneRestarter : MonoBehaviour
    {
        public KeyCode RestarterKeyCode = KeyCode.Backspace;

        /// <summary>
        /// On Update, looks for input
        /// </summary>
        protected virtual void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// Looks for a key press of the specified key
        /// </summary>
        protected virtual void HandleInput()
        {
            if (Input.GetKeyDown(RestarterKeyCode))
            {
                RestartScene();
            }
        }

        public virtual void RestartScene()
        {
            Debug.Log("Scene restarted by MMSceneRestarter");
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}

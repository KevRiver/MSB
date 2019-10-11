using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;

namespace MoreMountains.CorgiEngine
{
    public class DependencyInstaller : Editor
    {
        public class AfterImport : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (string importedAsset in importedAssets)
                {
                    if (importedAsset.Contains("DependencyInstaller.cs"))
                    {
                        WelcomeWindow.ShowWindow();
                    }
                }
            }
        }        

        public static void ReloadCurrentScene()
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(currentScenePath);
        }
    }
}

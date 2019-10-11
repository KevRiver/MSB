using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    public class SaveResetButton : MonoBehaviour
    {
        public virtual void ResetAllSaves()
        {
            SaveLoadManager.DeleteSaveFolder("MMAchievements");
            SaveLoadManager.DeleteSaveFolder("MMRetroAdventureProgress");
            SaveLoadManager.DeleteSaveFolder("InventoryEngine");
            SaveLoadManager.DeleteSaveFolder("CorgiEngine");
        }		
	}
}
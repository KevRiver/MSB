using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.SceneManagement;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A pickable one up, that gives you one extra life if picked up
    /// </summary>
    [AddComponentMenu("Corgi Engine/Items/Pickable One Up")]
    public class PickableOneUp : PickableItem
    {
        [Information("Add this component to an object with a Collider2D set as trigger, and it'll become pickable by Player Characters. When picked, it'll increase the amount of lives as specified. You can decide here to have only new lives added, within the limit of current lives containers, expand this limit, fill it accordingly, or fill all containers.", InformationAttribute.InformationType.Info, false)]
        [Header("Normal one ups")]
        public int NumberOfAddedLives;

        [Header("Containers")]
        public int NumberOfAddedEmptyContainers;
        public bool FillAddedContainers = false;
        public bool FillAllContainers = false;

        /// <summary>
        /// What happens when the object gets picked
        /// </summary>
        protected override void Pick()
        {
            GameManager.Instance.GainLives(NumberOfAddedLives);

            GameManager.Instance.AddLives(NumberOfAddedEmptyContainers, FillAddedContainers);

            if (FillAllContainers)
            {
                GameManager.Instance.GainLives(GameManager.Instance.MaximumLives);
            }

        }
    }
}
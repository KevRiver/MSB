using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoreMountains.Tools
{
    public class MMObjectPool : MonoBehaviour
    {
        [ReadOnly]
        public List<GameObject> PooledGameObjects;
    }
}

using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// A zipline will allow you to travel between two poles (from Pole1 to Pole2) using a grip.
    /// It requires a little bit of setup, as you'll need two poles, a line renderer, a grip, and all that bound together.
    /// You'll find an example of that in situation in the RetroForest demo scene.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Corgi Engine/Environment/Zipline")]
    public class Zipline : MonoBehaviour
    {
        /// the line renderer used to draw a line between the two poles
        public LineRenderer BoundLineRenderer;
        /// the Grip object (that should have both a Grip and MMPathMovement component on it)
        public MMPathMovement Grip;
        /// the object to use as the first pole
        public GameObject Pole1;
        /// the object to use as the second pole 
        public GameObject Pole2;
        /// the offset to apply from the pole's origins to draw the line renderer
        public Vector3 LineRendererOffset;
        /// the offset to apply from the pole's origins to draw the path the Grip will follow
        public Vector3 PathOffset;
        
        protected Vector3[] _polePositions = new Vector3[2];
        protected Vector3 _pole1;
        protected Vector3 _pole2;

        /// <summary>
        /// On start we prevent our Grip from moving
        /// </summary>
        protected virtual void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            Grip.CanMove = false;
        }

        /// <summary>
        /// On Update we draw a path and a line renderer between our two poles
        /// </summary>
        protected virtual void Update()
        {
            if ((BoundLineRenderer == null) || (Pole1 == null) || (Pole2 == null))
            {
                return;
            }

            _pole1 = Pole1.transform.position + LineRendererOffset;
            _pole2 = Pole2.transform.position + LineRendererOffset;
            _polePositions[0] = _pole1;
            _polePositions[1] = _pole2;

            BoundLineRenderer.SetPositions(_polePositions);

            if (Grip != null)
            {
                Grip.PathElements[0].PathElementPosition = Pole1.transform.position - this.transform.position + PathOffset;
                Grip.PathElements[1].PathElementPosition = Pole2.transform.position - this.transform.position  + PathOffset;
            }
        }
    }
}

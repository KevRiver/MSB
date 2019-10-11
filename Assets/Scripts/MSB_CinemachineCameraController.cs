using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

public class MSB_CinemachineCameraController : CinemachineCameraController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        if ((_confiner != null) && ConfineCameraToLevelBounds)
        {
            _confiner.m_BoundingVolume = MSB_LevelManager.Instance.BoundsCollider;
        }
        if (UseOrthographicZoom)
        {
            _virtualCamera.m_Lens.OrthographicSize = InitialOrthographicZoom;
        }
        if (UsePerspectiveZoom)
        {
            SetPerspectiveZoom(InitialPerspectiveZoom);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

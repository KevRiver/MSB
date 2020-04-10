using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Nettention;
using UnityEngine.Experimental.PlayerLoop;

namespace Nettention.Proud
{
    

}
public class DRM : MonoBehaviour
{
    private Nettention.Proud.PositionFollower _positionFollower;

    public virtual void MoveRemote(Vector3 pos,Vector3 vel,Quaternion rot)
    {
        var npos = new Nettention.Proud.Vector3();
        npos.x = pos.x;
        npos.y = pos.y;
        npos.z = pos.z;
        
        var nvel = new Nettention.Proud.Vector3();
        nvel.x = vel.x;
        nvel.y = vel.y;
        nvel.z = vel.z;
        _positionFollower.SetTarget(npos,nvel);
        transform.rotation = rot;
    }

    protected virtual void FollowRemoteMove()
    {
        _positionFollower.FrameMove(Time.deltaTime);
        var pos = new Nettention.Proud.Vector3();
        var velocity = new Nettention.Proud.Vector3();
        _positionFollower.GetFollower(ref pos, ref velocity);
        transform.position = new Vector3((float)pos.x,(float)pos.y,(float)pos.z);
    }

    private void Update()
    {
        FollowRemoteMove();
    }
}

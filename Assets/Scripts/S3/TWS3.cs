using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TWS3 : Bullet
{
    [SerializeField] internal float2 rot; //initRot, endRot
    internal static TWS3Ctl castedCtl;
    protected override void Start()
    {
        castedCtl.CustomAdd(this);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.CustomRemove(this);
    }
}

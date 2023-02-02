using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PS2 : Bullet
{
    static internal PS2Ctl castedCtl;
    internal readonly float startingPhase = 1.570796f;
    protected override void Start()
    {
        castedCtl.CustomAdd(this, startingPhase, transform.rotation.eulerAngles.z);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.CustomRemove(this);
    }
}

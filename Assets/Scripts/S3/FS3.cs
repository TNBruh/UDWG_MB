using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS3 : Bullet
{
    static internal FS3Ctl castedCtl;
    internal float fallSpeed;
    internal float rotSpeed;

    protected override void Start()
    {
        castedCtl.CustomAdd(this);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.CustomRemove(this);
    }
}

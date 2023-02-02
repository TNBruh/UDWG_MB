using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C1S3 : Bullet
{
    internal float prog = 0;
    static internal C1S3Ctl castedCtl;

    protected override void Start()
    {
        castedCtl.CustomAdd(this);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.CustomRemove(this);
    }
}

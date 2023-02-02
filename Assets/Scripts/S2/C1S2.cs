using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C1S2 : Bullet
{
    static internal C1S2Ctl castedCtl;

    protected override void Start()
    {
        castedCtl.Add(this);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

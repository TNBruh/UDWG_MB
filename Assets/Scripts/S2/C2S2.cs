using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C2S2 : Bullet
{
    static internal C2S2Ctl castedCtl;

    protected override void Start()
    {
        castedCtl.Add(this);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

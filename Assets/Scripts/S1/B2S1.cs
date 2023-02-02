using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class B2S1 : Bullet
{
    static internal BS1Ctl castedCtl;
    protected override void Start()
    {
        castedCtl.Add(this);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

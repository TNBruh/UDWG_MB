using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1S3 : Bullet
{
    static internal B1S3Ctl castedCtl;
    protected override void Start()
    {
        castedCtl.Add(this);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

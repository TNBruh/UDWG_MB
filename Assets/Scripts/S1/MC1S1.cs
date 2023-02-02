using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC1S1 : Bullet
{
    [SerializeField] internal float dir; // +/- 25
    static internal MC1S1Ctl castedCtl;

    protected override void Start()
    {
        castedCtl.CustomAdd(this, dir);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.CustomRemove(this);
    }
}

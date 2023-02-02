using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TS3 : Bullet
{
    [SerializeField]
    Vector3[] pos = new Vector3[2] //startPos, endPos
    {
        new Vector3(0, 6, 0),
        new Vector3(0, 4, 0),
    };
    static internal TS3Ctl castedCtl;
    protected override void Start()
    {
        castedCtl.Add(this);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

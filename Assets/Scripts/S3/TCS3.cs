using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCS3 : Bullet
{
    static internal TCS3Ctl castedCtl;
    [SerializeField] internal float waveSpeed = 4;
    internal float prog = 1.570796f;
    protected override void Start()
    {
        castedCtl.CustomAdd(this);
    }

    private void Update()
    {
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {

    }
}

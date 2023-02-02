using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES3 : Bullet
{
    static internal ES3Ctl castedCtl;
    
    internal float rndFallDeviation;
    protected override void Start()
    {
        castedCtl.CustomAdd(this);
    }

    
}

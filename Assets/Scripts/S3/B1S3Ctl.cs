using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1S3Ctl : BulletCtl
{
    [SerializeField] GameObject bPrefab;
    private void Start()
    {
        B1S3.bulletCtl = this;
        B1S3.castedCtl = this;
    }
    internal override void Update()
    {
        MoveBullet();
    }

    internal void Spawn(Vector3 pos, Quaternion rot)
    {
        Instantiate(bPrefab, pos, rot);
    }
}

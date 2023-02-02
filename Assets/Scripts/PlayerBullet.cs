using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    static internal PlayerBulletCtl castedCtl;
    protected override void Start()
    {
        //if (bulletCtl == null) bulletCtl = FindObjectOfType(typeof(BulletCtl)) as BulletCtl;
        castedCtl.Add(this);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

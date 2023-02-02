using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine.Jobs;

public class PlayerBulletCtl : BulletCtl
{
    [SerializeField] GameObject bulletPrefab;
    private void Start()
    {
        PlayerBullet.bulletCtl = this;
        PlayerBullet.castedCtl = this;
    }

    internal override void Update()
    {
        MoveBullet();
    }

    internal virtual void Spawn(Vector3 tl, Quaternion rot)
    {
        //decided to not utilize pooling because the ECS version doesn't use it yet still performs well
        //trying hard to keep their approach as close as possible despite having different architecture
        GameObject bullet = Instantiate(bulletPrefab, tl, rot);
        //Add(bullet.GetComponent(typeof(PlayerBullet)) as PlayerBullet);

    }
}

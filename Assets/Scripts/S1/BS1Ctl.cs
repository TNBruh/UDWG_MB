using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Jobs;

public class BS1Ctl : BulletCtl
{
    S1Manager s1Manager;
    [SerializeField] GameObject b1Prefab;
    private void Start()
    {
        B1S1.bulletCtl = this;
        B2S1.bulletCtl = this;
        B1S1.castedCtl = this;
        B2S1.castedCtl = this;
        s1Manager = FindObjectOfType(typeof(S1Manager)) as S1Manager;
    }
    internal override void Update()
    {
        TransformAccessArray transformAccessArray = new TransformAccessArray(bulletTfsList.ToArray());

        //execute job
        MoveBulletJob moveBulletJob = new MoveBulletJob
        {
            deltaTime = Time.deltaTime,
            speed = speed,
        };

        moveBulletJob.Schedule(transformAccessArray).Complete();
        transformAccessArray.Dispose();
    }


    internal void Spawn()
    {
        List<Transform> mcTfs = s1Manager.mc1ctl.bulletTfsList;
        foreach (Transform t in mcTfs)
        {
            if (!TransformUtil.IsInBarrier(t.position)) continue;
            for (int i = 0; i < 4; i++)
            {
                Instantiate(b1Prefab, t.position, t.rotation * Quaternion.Euler(0, 0, i * 90));
            }
        }

    }

    internal override void Clear()
    {
        int c = bulletList.Count();
        for (int i = c - 1; i >= 0; i--)
        {
            Destroy(bulletList[i].gameObject);
        }
        bulletList.Clear();
        bulletTfsList.Clear();
    }
}

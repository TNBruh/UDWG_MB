using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;

public class C2S2Ctl : BulletCtl
{
    [SerializeField] GameObject c2Prefab;
    [SerializeField] GameObject pPrefab;
    internal bool isFrozen = true;
    static internal readonly float spawnDist = 0.5f;
    S2Manager s2Manager;

    internal override void Update()
    {
        TransformAccessArray tfArr = BulletAccessArray;

        Movejob job = new Movejob
        {
            deltatime = Time.deltaTime,
            frozenMult = isFrozen ? 0 : 1,
            speed = speed,
        };
        job.Schedule(tfArr).Complete();

        tfArr.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        s2Manager = GameObject.FindObjectOfType(typeof(S2Manager)) as S2Manager;
        C2S2.bulletCtl = this;
        C2S2.castedCtl = this;
    }

    internal void Spawn(Vector3 pos)
    {
        float spawnRad = UnityEngine.Random.value * math.PI * 2;
        float xDist;
        float yDist;
        math.sincos(spawnRad, out xDist, out yDist);
        Vector3 spawnPos = new Vector3(xDist, yDist, 0) * spawnDist * UnityEngine.Random.value;

        float eulerRot = math.degrees(spawnRad);

        Instantiate(c2Prefab, spawnPos + pos, Quaternion.Euler(0,0, eulerRot));
    }

    internal void Melt()
    {
        int count = bulletList.Count;
        //foreach (Transform t in bulletTfsList)
        //{
        //    if (!TransformUtil.IsInBarrier(t.position)) continue;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        bool isPepe = UnityEngine.Random.value <= 0.25f;
        //        if (isPepe)
        //        {
        //            Instantiate(p, t.position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360));
        //        } else
        //        {
        //            Instantiate(c2, t.position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360));
        //        }
        //    }
        //}
        for (int i = count - 1; i >= 0; i--)
        {
            if (!TransformUtil.IsInBarrier(bulletTfsList[i].position)) continue;
            for (int j = 0; j < 2; j++)
            {
                bool isPepe = UnityEngine.Random.value <= 0.25f;
                if (isPepe)
                {
                    Instantiate(pPrefab, bulletTfsList[i].position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360));
                }
                else
                {
                    Instantiate(c2Prefab, bulletTfsList[i].position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360));
                }
            }
            Destroy(bulletList[i].gameObject);
            bulletList.RemoveAt(i);
            bulletTfsList.RemoveAt(i);
        }
        isFrozen = false;
    }

    internal void Freeze()
    {
        isFrozen = true;
    }


    protected struct Movejob : IJobParallelForTransform
    {
        [ReadOnly]
        public float speed;
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float frozenMult;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += (Vector3)TransformUtil.RotateVector(new float3(0, speed * frozenMult * deltatime, 0), math.radians(transform.rotation.eulerAngles.z));
        }
    }
}

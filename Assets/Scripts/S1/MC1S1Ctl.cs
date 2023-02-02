using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using System;
using System.Linq;

public class MC1S1Ctl : BulletCtl
{
    [SerializeField] TimerUtil recoilTimer;
    [SerializeField] GameObject mcPrefab;
    [SerializeField] float rotSpeed;
    S1Manager s1Manager;
    public static readonly float[] dirs = new float[]
    {
        65, 115, -115, -65
    };
    internal List<float> bulletDirs = new List<float>();
    public static GameObject[] mcTemplates;
    static float eulerRot = 0;

    float[] MCDirs
    {
        get
        {
            //float[] res = new float[bulletList.Count];
            //for (int i = 0; i < res.Length; i++)
            //{
            //    res[i] = (bulletList[i] as MC1S1).dir;
            //}
            //return res;
            return bulletDirs.ToArray();
        }
    }

    internal void CustomAdd(Bullet bullet, float dir)
    {
        this.Add(bullet);
        bulletDirs.Add(dir);
    }

    internal void CustomRemove(Bullet bullet)
    {
        int ind = bulletList.IndexOf(bullet);
        //Debug.Log(ind);
        //Debug.Log(bulletDirs[ind]);
        //Debug.Log(bulletList[ind]);
        //Debug.Log(bulletTfsList[ind].position);
        bulletDirs.RemoveAt(ind);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        Destroy(bullet.gameObject);
    }

    private void Start()
    {
        s1Manager = FindObjectOfType(typeof(S1Manager)) as S1Manager;
        MC1S1.bulletCtl = this;
        MC1S1.castedCtl = this;

        mcTemplates = new GameObject[dirs.Length];
        for (int i = 0; i < dirs.Length; i++)
        {
            mcTemplates[i] = Instantiate(mcPrefab);
            MC1S1 script = mcTemplates[i].GetComponent(typeof(MC1S1)) as MC1S1;
            script.dir = dirs[i];
            mcTemplates[i].transform.rotation = Quaternion.Euler(0, 0, i * 90);
            mcTemplates[i].SetActive(false);
        }
    }

    internal void Spawn()
    {
        for (int i = 0; i < 4; i++)
        {
            Instantiate(mcTemplates[i], s1Manager.npcCtl.transform.position, s1Manager.transform.rotation).SetActive(true);
        }
    }

    internal override void Clear()
    {
        //base.Clear();
        int c = bulletList.Count();
        for (int i = c-1; i >= 0; i--)
        {
            Destroy(bulletList[i].gameObject);
        }
        bulletList.Clear();
        bulletTfsList.Clear();
        bulletDirs.Clear();
        for (int i = 0; i < mcTemplates.Length; i++) Destroy(mcTemplates[i]);
    }

    internal override void Update()
    {
        eulerRot = (eulerRot + rotSpeed * Time.deltaTime) % 360;
        TransformAccessArray transformAccessArray = new TransformAccessArray(bulletTfsList.ToArray());
        NativeArray<float> arrDirs = new NativeArray<float>(MCDirs.ToArray(), Allocator.TempJob);

        MoveJob job = new MoveJob
        {
            deltatime = Time.deltaTime,
            dirs = arrDirs,
            globalRot = eulerRot,
            speed = speed,
        };

        job.Schedule(transformAccessArray).Complete();
        transformAccessArray.Dispose();
        arrDirs.Dispose();

        List<Bullet> toDispose = new List<Bullet>();
        foreach (Bullet b in bulletList)
        {
            if (Vector3.Distance(Vector3.zero, b.transform.position) >= 7)
            {
                toDispose.Add(b);
            }
        }
        toDispose.ForEach((Bullet b) =>
        {
            CustomRemove(b);
        });
    }

    protected struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float speed;
        [ReadOnly]
        public float globalRot;
        [ReadOnly]
        public NativeArray<float> dirs;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += (Vector3)TransformUtil.RotateVector(new float3(0, speed * deltatime, 0), math.radians(dirs[index]));
            transform.rotation = Quaternion.Euler(0, 0, globalRot);
        }
    }
}

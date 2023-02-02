using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class ES3Ctl : BulletCtl
{
    [SerializeField] GameObject ePrefab;

    [SerializeField] internal float stormFallDeviation;
    [SerializeField] internal float stormFallSpeed;
    [SerializeField] internal float rotSpeed;
    
    internal List<float> rndFallDeviations = new List<float>();

    private void Start()
    {
        ES3.castedCtl = this;
        ES3.bulletCtl = this;
    }
    internal override void Update()
    {

        TransformAccessArray tfArr = BulletAccessArray;
        NativeArray<float> fallDeviations = new NativeArray<float>(rndFallDeviations.ToArray(), Allocator.TempJob);
        NativeArray<bool> markDestruction = new NativeArray<bool>(tfArr.length, Allocator.TempJob);
        //NativeArray<float> fallSpeeds = new NativeArray<float>(rndFallSpeeds.ToArray(), Allocator.TempJob);

        MoveJob job = new MoveJob
        {
            deltatime = Time.deltaTime,
            rndFallDeviation = fallDeviations,
            rndFallSpeed = speed,
            rotSpeed = rotSpeed,
            markDestruction = markDestruction,
        };
        job.Schedule(tfArr).Complete();


        tfArr.Dispose();
        fallDeviations.Dispose();


        for (int i = 0; i < bulletList.Count; i++)
        {
            if (markDestruction[i]) CustomRemove((ES3)bulletList[i]);
        }
        markDestruction.Dispose();
    }

    internal void CustomAdd(ES3 bullet)
    {
        Add(bullet);
        rndFallDeviations.Add(bullet.rndFallDeviation);
        //rndFallSpeeds.Add(bullet.rndFallSpeed);
    }

    internal void CustomRemove(ES3 bullet)
    {
        int ind = bulletList.IndexOf(bullet);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        //rndFallSpeeds.RemoveAt(ind);
        rndFallDeviations.RemoveAt(ind);
        Destroy(bullet.gameObject);
    }

    internal override void Clear()
    {
        int count = bulletList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(bulletList[i].gameObject);
        }
        bulletList.Clear();
        bulletTfsList.Clear();
        rndFallDeviations.Clear();
    }

    internal void SpawnSet(Vector3[] positions)
    {
        float deviation = math.lerp(-stormFallDeviation, stormFallDeviation, UnityEngine.Random.value) + 180;
        Quaternion rndRot = Quaternion.Euler(0, 0, UnityEngine.Random.value * 90);
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject b = Instantiate(ePrefab, positions[i], rndRot);
            ES3 e = b.GetComponent(typeof(ES3)) as ES3;
            e.rndFallDeviation = deviation * (i % 2 == 0 ? -1 : 1);
        }
    }

    protected struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float rotSpeed;
        [ReadOnly]
        public NativeArray<float> rndFallDeviation;
        [ReadOnly]
        public float rndFallSpeed;
        [NativeDisableParallelForRestriction]
        public NativeArray<bool> markDestruction;
        
        public void Execute(int index, TransformAccess transform)
        {
            transform.position += (Vector3)TransformUtil.RotateVector(new Vector3(0, rndFallSpeed * deltatime, 0), math.radians(rndFallDeviation[index]));

            transform.rotation *= Quaternion.Euler(0, 0, rotSpeed * deltatime);

            if (Vector3.Distance(transform.position, Vector3.zero) >= 10) markDestruction[index] = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class FS3Ctl : BulletCtl
{
    [SerializeField] GameObject fPrefab;

    internal List<float> rotSpeeds = new List<float>();
    internal List<float> fallSpeeds = new List<float>();
    [SerializeField] float2 rotSpeedRange;
    [SerializeField] float2 fallSpeedRange;
    [SerializeField]
    float2[] spawnPosRange = new float2[2]
    {
        float2.zero,
        float2.zero
    };

    private void Start()
    {
        FS3.castedCtl = this;
        FS3.bulletCtl = this;
    }
    internal override void Update()
    {
        //if (Input.GetButton("Fire1")) Spawn();
        MoveJobWrapper();
    }

    internal void CustomAdd(FS3 bullet)
    {
        Add(bullet);
        rotSpeeds.Add(bullet.rotSpeed);
        fallSpeeds.Add(bullet.fallSpeed);
    }

    internal void CustomRemove(FS3 bullet)
    {
        int ind = bulletList.IndexOf(bullet);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        rotSpeeds.RemoveAt(ind);
        fallSpeeds.RemoveAt(ind);
        Destroy(bullet.gameObject);
    }

    internal void Spawn()
    {
        GameObject obj = Instantiate(fPrefab, new Vector2
        {
            x = math.lerp(spawnPosRange[0].x, spawnPosRange[1].x, UnityEngine.Random.value),
            y = math.lerp(spawnPosRange[0].y, spawnPosRange[1].y, UnityEngine.Random.value),
        },
        Quaternion.Euler(0,0,360 * UnityEngine.Random.value)
        );

        FS3 f = obj.GetComponent(typeof(FS3)) as FS3;
        f.rotSpeed = math.lerp(rotSpeedRange.x, rotSpeedRange.y, UnityEngine.Random.value);
        f.fallSpeed = math.lerp(fallSpeedRange.x, fallSpeedRange.y, UnityEngine.Random.value);
    }

    internal void MoveJobWrapper()
    {
        TransformAccessArray tfArr = BulletAccessArray;
        NativeArray<float> rotSpeed = new NativeArray<float>(rotSpeeds.ToArray(), Allocator.TempJob);
        NativeArray<float> fallSpeed = new NativeArray<float>(fallSpeeds.ToArray(), Allocator.TempJob);

        MoveJob job = new MoveJob
        {
            deltatime = Time.deltaTime,
            fallSpeed = fallSpeed,
            rotSpeed = rotSpeed,
        };

        job.Schedule(tfArr).Complete();
        tfArr.Dispose();
        rotSpeed.Dispose();
        fallSpeed.Dispose();
    }

    internal override void Clear()
    {
        rotSpeeds.Clear();
        fallSpeeds.Clear();
        bulletTfsList.Clear();
        int count = bulletList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(bulletList[i].gameObject);
        }
    }
    protected struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public NativeArray<float> rotSpeed;
        [ReadOnly]
        public NativeArray<float> fallSpeed;

        public void Execute(int index, TransformAccess transform)
        {
            transform.position += new Vector3
            {
                x = 0,
                y = -fallSpeed[index] * deltatime,
                z = 0
            };

            transform.rotation *= Quaternion.Euler(0, 0, rotSpeed[index] * deltatime);
        }
    }
}

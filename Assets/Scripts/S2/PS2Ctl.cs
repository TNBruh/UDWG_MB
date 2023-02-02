using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class PS2Ctl : BulletCtl
{
    [SerializeField] GameObject pPrefab;

    [SerializeField] internal float turnSpeed;
    [SerializeField] internal float2 turnBound = new float2(-60, 60);

    List<float> phases = new List<float>();
    List<float> originalRots = new List<float>();

    private void Start()
    {
        PS2.bulletCtl = this;
        PS2.castedCtl = this;
    }

    internal override void Update()
    {
        NativeArray<float> phasesArr = new NativeArray<float>(phases.ToArray(), Allocator.TempJob);
        NativeArray<float> originRotArr = new NativeArray<float>(originalRots.ToArray(), Allocator.TempJob);
        TransformAccessArray tfArr = BulletAccessArray;

        MoveJob job = new MoveJob
        {
            deltatime = Time.deltaTime,
            originalRots = originRotArr,
            phases = phasesArr,
            speed = speed,
            turnBound = turnBound,
            turnSpeed = turnSpeed,
        };
        job.Schedule(tfArr).Complete();

        phases = phasesArr.ToList();

        phasesArr.Dispose();
        originRotArr.Dispose();
        tfArr.Dispose();
    }

    internal void CustomAdd(Bullet b, float phase, float originRot)
    {
        Add(b);
        phases.Add(phase);
        originalRots.Add(originRot);
    }

    internal void CustomRemove(Bullet b)
    {
        int ind = bulletList.IndexOf(b);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        phases.RemoveAt(ind);
        originalRots.RemoveAt(ind);
        Destroy(b.gameObject);
    }

    internal override void Clear()
    {
        int c = bulletTfsList.Count;
        for (int i = c - 1; i >= 0; i--)
        {
            Destroy(bulletList[i].gameObject);
        }
        bulletList.Clear();
        bulletTfsList.Clear();
        phases.Clear();
        originalRots.Clear();
    }

    internal void Spawn(Vector3 pos, float eulerRot)
    {
        GameObject g = Instantiate(pPrefab, pos, Quaternion.Euler(0, 0, eulerRot));
    }
    /*
     
    readonly static internal float waitMelt = 1f;
    readonly static internal float waitFreeze = 7.2f;
    readonly static internal float waitIceCycle = 4f;
     */

    protected struct MoveJob : IJobParallelForTransform
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<float> phases;
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float turnSpeed;
        [ReadOnly]
        public float speed;
        [ReadOnly]
        public float2 turnBound;
        [ReadOnly]
        public NativeArray<float> originalRots;

        public void Execute(int index, TransformAccess transform)
        {
            phases[index] = (phases[index] + turnSpeed * deltatime) % (Mathf.PI * 2);
            float lerpProg = (math.sin(phases[index]) + 1) / 2;
            float lerpRotEuler = math.lerp(turnBound.x, turnBound.y, lerpProg);
            transform.rotation = Quaternion.Euler(0, 0, lerpRotEuler + originalRots[index]);

            transform.position += (Vector3)TransformUtil.RotateVector(new float3(0,speed*deltatime,0), math.radians(lerpRotEuler + originalRots[index]));

        }
    }
}

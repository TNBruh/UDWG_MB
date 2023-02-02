using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using System.Linq;

public class C1S3Ctl : BulletCtl
{
    List<float> progs = new List<float>();
    [SerializeField] float maxProg;
    internal void CustomAdd(C1S3 bullet)
    {
        Add(bullet);
        progs.Add(bullet.prog);
    }

    internal void CustomRemove(C1S3 bullet)
    {
        int ind = bulletList.IndexOf(bullet);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        progs.RemoveAt(ind);
        Destroy(bullet.gameObject);
    }

    private void Start()
    {
        C1S3.castedCtl = this;
    }
    
    internal override void Update()
    {
        MoveJobWrapper();
    }

    internal override void Clear()
    {
        progs.Clear();
        bulletTfsList.Clear();
        bulletList.Clear();
    }
    internal void MoveJobWrapper()
    {
        NativeArray<float> prog = new NativeArray<float>(progs.ToArray(), Allocator.TempJob);
        TransformAccessArray tfArr = BulletAccessArray;

        MoveJob job = new MoveJob
        {
            deltatime = Time.deltaTime,
            maxProg = maxProg,
            prog = prog,
            speed = speed,
        };
        job.Schedule(tfArr).Complete();

        progs = prog.ToList();

        prog.Dispose();
        tfArr.Dispose();
    }

    protected struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float speed;
        [ReadOnly]
        public float maxProg;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> prog;

        public void Execute(int index, TransformAccess transform)
        {
            if (maxProg <= prog[index]) return;
            prog[index] += deltatime;
            transform.position += deltatime * speed * (Vector3)TransformUtil.RotateVector(Vector3.up, math.radians(transform.rotation.eulerAngles.z));
        }
    }
}

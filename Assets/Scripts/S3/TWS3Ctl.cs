using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class TWS3Ctl : BulletCtl
{
    //intro
    [SerializeField] internal float wingProgSpeed = 1;
    internal bool intro = false;
    internal float introProg = 0;
    internal List<float2> rotRanges = new List<float2>();


    private void Start()
    {
        TWS3.castedCtl = this;
        TWS3.bulletCtl = this;
        introProg = 0;
    }

    internal void CustomAdd(TWS3 bullet)
    {
        Add(bullet);
        rotRanges.Add(bullet.rot);
        //waveSpeeds.Add(bullet.waveSpeed);
        //waveProgs.Add(bullet.prog);
    }

    internal void CustomRemove(TWS3 bullet)
    {
        int ind = bulletList.IndexOf(bullet);
        bulletList.RemoveAt(ind);
        bulletTfsList.RemoveAt(ind);
        rotRanges.RemoveAt(ind);
    }

    internal override void Update()
    {
        //if (Input.GetButtonDown("Fire1")) intro = true;
        if (intro)
        {
            IntroJobWrapper();
        }
    }

    internal void IntroJobWrapper()
    {
        introProg = math.clamp(introProg + wingProgSpeed * Time.deltaTime, 0, 1);
        TransformAccessArray tfArray = BulletAccessArray;
        NativeArray<float2> rotRange = new NativeArray<float2>(rotRanges.ToArray(), Allocator.TempJob);
        IntroJob job = new IntroJob
        {
            prog = introProg,
            rotRange = rotRange,
        };
        job.Schedule(tfArray).Complete();
        tfArray.Dispose();
        rotRange.Dispose();
        if (introProg >= 1) intro = false;
    }

    protected struct IntroJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float prog;
        [ReadOnly]
        public NativeArray<float2> rotRange;

        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = Quaternion.Euler(0, 0, math.lerp(rotRange[index].x, rotRange[index].y, prog));
        }
    }

    //data.wavePhase = (data.wavePhase + time* data.waveSpeed) % (2 * math.PI);
    //float lerpProg = (math.sin(data.wavePhase) + 1) / 2;
    //float lerpProgEuler = math.lerp(-S3SO.waveDeviation, S3SO.waveDeviation, lerpProg);
    //rotation.Value = SpellManagerMB.Degrees2Quaternion(lerpProgEuler + 180);
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using System.Linq;
using Unity.Mathematics;

public class TCS3Ctl : BulletCtl
{
    //wave
    [SerializeField] internal float waveDeviation = 3.8f;
    internal List<float> waveSpeeds = new List<float>();
    internal List<float> waveProgs = new List<float>();
    internal bool beginWave = false;
    private void Start()
    {
        TCS3.bulletCtl = this;
        TCS3.castedCtl = this;
    }
    internal override void Update()
    {
        //if (Input.GetButton("Fire1")) beginWave = true;
        if (beginWave)
        {
            WaveJobWrapper();
        }
    }

    internal void CustomAdd(TCS3 tc)
    {
        Add(tc);
        waveSpeeds.Add(tc.waveSpeed);
        waveProgs.Add(tc.prog);
    }

    internal void WaveJobWrapper()
    {
        NativeArray<float> waveSpeed = new NativeArray<float>(waveSpeeds.ToArray(), Allocator.TempJob);
        NativeArray<float> waveProg = new NativeArray<float>(waveProgs.ToArray(), Allocator.TempJob);
        TransformAccessArray tfArr = BulletAccessArray;
        WaveJob job = new WaveJob
        {
            deltatime = Time.deltaTime,
            waveDeviation = waveDeviation,
            waveSpeed = waveSpeed,
            prog = waveProg,
        };
        job.Schedule(tfArr).Complete();

        waveProgs = waveProg.ToList();

        waveSpeed.Dispose();
        waveProg.Dispose();
        tfArr.Dispose();
    }

    protected struct WaveJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltatime;
        [ReadOnly]
        public float waveDeviation;
        [ReadOnly]
        public NativeArray<float> waveSpeed;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> prog;

        public void Execute(int index, TransformAccess transform)
        {
            prog[index] = (prog[index] + deltatime * waveSpeed[index]) % (2 * math.PI);
            float sinProg = (math.sin(prog[index]) + 1) / 2;
            float lerpProgEuler = math.lerp(-waveDeviation, waveDeviation, sinProg);
            transform.rotation = Quaternion.Euler(0, 0, lerpProgEuler + 180);
        }
    }

    internal override void Clear()
    {
        bulletTfsList.Clear();
        bulletList.Clear();
        waveSpeeds.Clear();
        waveProgs.Clear();
    }
}

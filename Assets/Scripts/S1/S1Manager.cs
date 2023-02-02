using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class S1Manager : SpellManager
{
    internal NPCCtl npcCtl;
    [SerializeField] internal MC1S1Ctl mc1ctl;
    [SerializeField] internal BS1Ctl bs1ctl;
    [SerializeField] float mcRecoil;
    [SerializeField] float bRecoil;
    [SerializeField] TimerUtil bTimer;

    Coroutine mcCycle;
    Coroutine bCycle;
    public bool IsDone
    {
        get;
        set;
    } = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    internal IEnumerator Cycle() //4 seconds
    {
        while (!IsDone)
        {
            npcCtl.SetMovement(RandomNPCPosition());
            yield return new WaitUntil(() => npcCtl.IsOnDestination);
            mc1ctl.Spawn();
            yield return new WaitForSeconds(mcRecoil);
        }
    }

    internal IEnumerator BulletCycle() //2.4 time unit
    {
        yield return StartCoroutine(bTimer.Switch(true));
        while (true)
        {
            yield return new WaitUntil(() => bTimer.ConsumeCycle());
            bs1ctl.Spawn();
        }
    }

    internal static float3 RandomNPCPosition(float left = -2f, float right = 2f, float bottom = 0, float top = 3)
    {
        return new float3
        {
            x = UnityEngine.Random.Range(left, right),
            y = UnityEngine.Random.Range(bottom, top),
            z = 0
        };
    }

    internal override IEnumerator Init()
    {
        npcCtl = GameObject.FindObjectOfType(typeof(NPCCtl)) as NPCCtl;
        bTimer.Init(bRecoil);

        bCycle = StartCoroutine(BulletCycle());
        mcCycle = StartCoroutine(Cycle());
        yield break;
    }

    internal override bool Finished()
    {
        return npcCtl.health <= 0;
    }

    internal override IEnumerator CleanUp()
    {
        StopCoroutine(bCycle);
        StopCoroutine(mcCycle);
        yield return new WaitForFixedUpdate();
        mc1ctl.Clear();
        bs1ctl.Clear();
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Net;

public class S2Manager : SpellManager
{
    internal NPCCtl npcCtl;
    internal PlayerCtl playerCtl;
    [SerializeField] internal C1S2Ctl c1Ctl;
    [SerializeField] internal C2S2Ctl c2Ctl;
    [SerializeField] internal PS2Ctl pCtl;
    [SerializeField] float waitMelt;
    [SerializeField] float waitFreeze;
    [SerializeField] float waitIceCycle;
    [SerializeField] float waitBlossomRecoil;
    [SerializeField] int iceFireCount = 1;
    [SerializeField] TimerUtil c1Timer;
    [SerializeField] TimerUtil c2Timer;

    Coroutine blossomSpawnCycle;
    Coroutine icePhaseCycle;
    Coroutine cycle;

    internal override IEnumerator CleanUp()
    {
        if (blossomSpawnCycle != null) StopCoroutine(blossomSpawnCycle);
        if (icePhaseCycle != null) StopCoroutine(icePhaseCycle);
        StopCoroutine(cycle);
        yield return new WaitForFixedUpdate();
        c1Ctl.Clear();
        c2Ctl.Clear();
        pCtl.Clear();
    }

    internal override bool Finished()
    {
        return npcCtl.health <= 0;
    }

    internal override IEnumerator Init()
    {
        npcCtl = GameObject.FindObjectOfType(typeof(NPCCtl)) as NPCCtl;
        playerCtl = GameObject.FindObjectOfType(typeof(PlayerCtl)) as PlayerCtl;

        cycle = StartCoroutine(Cycle());

        yield break;
    }

    internal IEnumerator Cycle()
    {
        while (true)
        {
            icePhaseCycle = StartCoroutine(IceFireCycle());
            yield return icePhaseCycle;
            c2Ctl.Melt();
            yield return new WaitUntil(() => !c2Ctl.isFrozen);
            yield return new WaitForSeconds(waitFreeze);
            c2Ctl.Freeze();
            yield return new WaitForFixedUpdate();
        }
    }

    internal IEnumerator IceFireCycle()
    {
        blossomSpawnCycle = StartCoroutine(BlossomCycle());
        for (int i = 0; i < iceFireCount; i++)
        {
            npcCtl.SetMovement(RandomNPCPosition(bottom: 1.4f));
            yield return new WaitUntil(() => npcCtl.IsOnDestination);
            yield return new WaitForFixedUpdate();

            c1Ctl.Spawn();
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(waitIceCycle);
        }
        StopCoroutine(blossomSpawnCycle);
    }

    internal IEnumerator BlossomCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitBlossomRecoil);
            c1Ctl.bulletTfsList.ForEach((t) =>
            {
                if (!TransformUtil.IsInBarrier(t.position)) return;
                for (int i = 0; i < 3; i++)
                {
                    c2Ctl.Spawn(t.position);
                }
            });
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
    /*
    IEnumerator Spell2Cycle()
    {
        while (true)
        {
            yield return StartCoroutine(IceFireCycle());
            yield return new WaitForSeconds(S2SO.waitMelt);
            S2SO.melt = true;
            yield return new WaitWhile(() => S2SO.melt);
            yield return new WaitForSeconds(S2SO.waitFreeze);
            S2SO.freeze = true;
            yield return new WaitWhile(() => S2SO.freeze);
        }
    }

    IEnumerator IceFireCycle()
    {
        Coroutine blossomCycle = StartCoroutine(BlossomSpawnCycle());
        for (int i = 0; i < S2SO.iceFireCount; i++)
        {
            //randomizes next movement
            float3 nextPos = RandomNPCPosition(bottom: 1.4f);
            NPCSystem.SetMovement(nextPos);

            //waits until lerp finishes
            yield return new WaitUntil(() => NPCSystem.lerpProg == 1);
            yield return new WaitForFixedUpdate();

            //fire
            S2SO.c1Fire = true;
            //recoil
            yield return new WaitForSeconds(S2SO.waitIceCycle);
        }
        StopCoroutine(blossomCycle);
    }

    IEnumerator BlossomSpawnCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(S2SO.blossomRecoil);
            S2SO.c2Fire = true;
        }
    }
     */
}

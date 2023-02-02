using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class S3Manager : SpellManager
{
    internal NPCCtl npcCtl;
    [SerializeField] B1S3Ctl b1Ctl;
    [SerializeField] C1S3Ctl c1Ctl;
    [SerializeField] ES3Ctl eCtl;
    [SerializeField] FS3Ctl fCtl;
    [SerializeField] TCS3Ctl tcCtl;
    [SerializeField] TS3Ctl tCtl;
    [SerializeField] TWS3Ctl twCtl;

    [SerializeField] Vector3 npcPos = new Vector3(0, 2.5f, 0);

    [SerializeField] float eStormRecoil;
    [SerializeField] float fRecoil;
    [SerializeField] float bRecoil;

    Coroutine cycle;
    Coroutine tcCycle;
    Coroutine twIntro;
    Coroutine fCycle;
    Coroutine eCycle;
    Coroutine bCycle;


    internal override IEnumerator CleanUp()
    {
        if (cycle != null) StopCoroutine(cycle);
        if (tcCycle != null) StopCoroutine(tcCycle);
        if (twIntro != null) StopCoroutine(twIntro);
        if (fCycle != null) StopCoroutine(fCycle);
        if (eCycle != null) StopCoroutine(eCycle);
        if (bCycle != null) StopCoroutine(bCycle);

        b1Ctl.Clear();
        eCtl.Clear();
        fCtl.Clear();
        tCtl.Clear();

        yield return new WaitForFixedUpdate();
    }

    internal override bool Finished()
    {
        return npcCtl.health <= 0;
    }

    internal override IEnumerator Init()
    {
        npcCtl = GameObject.FindObjectOfType(typeof(NPCCtl)) as NPCCtl;

        cycle = StartCoroutine(Cycle());
        yield break;
    }

    IEnumerator Cycle()
    {
        yield return StartCoroutine(Intro());
        fCycle = StartCoroutine(FCycle());
        tcCtl.beginWave = true;
        eCycle = StartCoroutine(ECycle());
    }

    IEnumerator Intro()
    {
        npcCtl.SetMovement(npcPos);
        yield return new WaitUntil(() => npcCtl.IsOnDestination);
        tCtl.Spawn();
        yield return new WaitUntil(() => tCtl.FinishedIntro);
        bCycle = StartCoroutine(BCycle());
        yield return StartCoroutine(TWIntro());
    }

    IEnumerator TWIntro()
    {
        twCtl.intro = true;
        yield return new WaitWhile(() => twCtl.intro);
    }

    //IEnumerator TCCycle()
    //{
    //    tcCtl.beginWave = true;
    //}

    IEnumerator ECycle()
    {
        Vector3[] eSpawnPosition = tcCtl.bulletTfsList.Select((t) => t.position).ToArray();
        while (true)
        {
            eCtl.SpawnSet(eSpawnPosition);
            yield return new WaitForSeconds(eStormRecoil);
        }
    }

    IEnumerator FCycle()
    {
        while (true)
        {
            for (int i = 0; i < 6; i++)
            {
                fCtl.Spawn();
            }
            yield return new WaitForSeconds(fRecoil);
        }
    }

    IEnumerator BCycle()
    {
        Transform[] bSpawnTf = twCtl.bulletTfsList.ToArray().Concat(tcCtl.bulletTfsList).ToArray();
        while (true)
        {
            foreach (Transform tf in bSpawnTf)
            {
                b1Ctl.Spawn(tf.position, tf.rotation);
            }
            yield return new WaitForSeconds(bRecoil);
        }
    }
}

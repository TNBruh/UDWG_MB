using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Assertions.Must;

public class StageManager : MonoBehaviour
{
    [SerializeField] S1Manager s1Prefab;
    S1Manager s1Manager;
    [SerializeField] S2Manager s2Prefab;
    S2Manager s2Manager;
    [SerializeField] S3Manager s3Prefab;
    S3Manager s3Manager;
    [SerializeField] GameObject npcPrefab;
    NPCCtl npcCtl;
    [SerializeField] GameObject playerPrefab;
    PlayerCtl playerCtl;

    GameObject loadingCanvasObj;

    Coroutine cycle;

    PauseCtl pauseCtl;

    GameManager gameManager;

    DialogueMB dialogueMB;

    SpellNameMB spellNameMB;

    ShadowMB mokouS;
    ShadowMB cirnoS;
    ShadowMB byakurenS;

    //spell names
    static internal readonly string[] spellNames = new string[3]
    {
        "Transcribe \"Fury Beyond the Samsara of Life and Death\"",
        "Transcribe \"Brilliant Blizzard Blossom\"",
        "Transcribe \"Absent Desire to Welcome the Eight Sufferings\"",
    };

    public static readonly bool isTesting = true;
    public static readonly float spellTestLength = 30;
    public static int spellPhase = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        spellPhase = 0;
        npcCtl = Instantiate(npcPrefab).GetComponent(typeof(NPCCtl)) as NPCCtl;
        playerCtl = Instantiate(playerPrefab).GetComponent(typeof(PlayerCtl)) as PlayerCtl;
        npcCtl.SetInvuln(true);

        gameManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        loadingCanvasObj = GameObject.FindGameObjectWithTag("LoadingCanvas");
        loadingCanvasObj.SetActive(true);
        pauseCtl = GameObject.FindObjectOfType(typeof(PauseCtl), true) as PauseCtl;
        pauseCtl.gameObject.SetActive(false);
        pauseCtl.Init(gameManager.EnterMain);
        

        //s1Manager
        s1Manager = Instantiate(s1Prefab).GetComponent(typeof(S1Manager)) as S1Manager;
        //s2Manager
        s2Manager = Instantiate(s2Prefab).GetComponent(typeof(S2Manager)) as S2Manager;
        //s3Manager
        s3Manager = Instantiate(s3Prefab).GetComponent(typeof(S3Manager)) as S3Manager;

        //dialogue mb
        dialogueMB = GameObject.FindObjectOfType(typeof(DialogueMB)) as DialogueMB;

        spellNameMB = GameObject.FindObjectOfType(typeof(SpellNameMB)) as SpellNameMB;

        mokouS = GameObject.FindGameObjectWithTag("mokouS").GetComponent(typeof(ShadowMB)) as ShadowMB;
        cirnoS = GameObject.FindGameObjectWithTag("cirnoS").GetComponent(typeof(ShadowMB)) as ShadowMB;
        byakurenS = GameObject.FindGameObjectWithTag("byakurenS").GetComponent(typeof(ShadowMB)) as ShadowMB;

        mokouS.gameObject.SetActive(false);
        cirnoS.gameObject.SetActive(false);
        byakurenS.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !loadingCanvasObj.activeSelf)
        {
            if (pauseCtl.gameObject.activeSelf)
            {
                pauseCtl.Continue();
            } else
            {
                Time.timeScale = 0;
                pauseCtl.gameObject.SetActive(true);
            }
        }
    }

    internal Coroutine Begin()
    {
        cycle = StartCoroutine(Stage());
        return cycle;
    }

    internal IEnumerator Stage()
    {
        loadingCanvasObj.SetActive(false);
        if (!isTesting)
        {
            dialogueMB.BeginDialogue(DialogueSO.startDialogue);
            yield return new WaitWhile(() => dialogueMB.onDialogue);

            //start actual fight
            mokouS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[0]);
            npcCtl.SetInvuln(false);
            spellPhase = 1;
            yield return StartCoroutine(S1API());
            mokouS.NextPath();
            npcCtl.Regenerate();
            yield return new WaitUntil(() => mokouS.prog >= 1);
            yield return new WaitUntil(() => npcCtl.IsMaxHealth);

            cirnoS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[1]);
            npcCtl.SetInvuln(false);
            spellPhase = 2;
            yield return StartCoroutine(S2API());
            cirnoS.NextPath();
            npcCtl.Regenerate();
            yield return new WaitUntil(() => cirnoS.prog >= 1);
            yield return new WaitUntil(() => npcCtl.IsMaxHealth);

            byakurenS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[2]);
            npcCtl.SetInvuln(false);
            spellPhase = 3;
            yield return StartCoroutine(S3API());
            byakurenS.NextPath();
            yield return new WaitUntil(() => byakurenS.prog >= 1);
            npcCtl.SetInvuln(true);

            dialogueMB.BeginDialogue(DialogueSO.endDialogue);
            yield return new WaitWhile(() => dialogueMB.onDialogue);
        } else
        {

            //start actual fight
            mokouS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[0]);
            npcCtl.SetInvuln(false);
            spellPhase = 1;
            yield return StartCoroutine(S1API());
            mokouS.NextPath();
            npcCtl.Regenerate();
            yield return new WaitUntil(() => mokouS.prog >= 1);
            yield return new WaitUntil(() => npcCtl.IsMaxHealth);

            cirnoS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[1]);
            npcCtl.SetInvuln(false);
            spellPhase = 2;
            yield return StartCoroutine(S2API());
            cirnoS.NextPath();
            npcCtl.Regenerate();
            yield return new WaitUntil(() => cirnoS.prog >= 1);
            yield return new WaitUntil(() => npcCtl.IsMaxHealth);

            byakurenS.gameObject.SetActive(true);
            spellNameMB.SetText(spellNames[2]);
            npcCtl.SetInvuln(false);
            spellPhase = 3;
            yield return StartCoroutine(S3API());
            byakurenS.NextPath();
            yield return new WaitUntil(() => byakurenS.prog >= 1);
            npcCtl.SetInvuln(true);

            Coroutine clearname = spellNameMB.SetText("692077616e7420746f20646965");
            yield return clearname;

            BenchMB.SendStats();

        }

        gameManager.EnterMain();
    }

    internal IEnumerator S1API()
    {
        yield return StartCoroutine(s1Manager.Init());
        if (!isTesting)
        {
            yield return new WaitUntil(() => s1Manager.Finished());
        } else
        {
            yield return new WaitForSeconds(spellTestLength);
        }
        yield return StartCoroutine(s1Manager.CleanUp());

    }

    internal IEnumerator S2API()
    {
        yield return StartCoroutine(s2Manager.Init());
        if (!isTesting)
        {
            yield return new WaitUntil(() => s2Manager.Finished());
        }
        else
        {
            yield return new WaitForSeconds(spellTestLength);
        }
        yield return StartCoroutine(s2Manager.CleanUp());
    }

    internal IEnumerator S3API()
    {
        yield return StartCoroutine(s3Manager.Init());
        if (!isTesting)
        {
            yield return new WaitUntil(() => s3Manager.Finished());
        }
        else
        {
            yield return new WaitForSeconds(spellTestLength);
        }
        yield return StartCoroutine(s3Manager.CleanUp());
    }

    internal IEnumerator CleanUp()
    {
        loadingCanvasObj.SetActive(true);
        pauseCtl.gameObject.SetActive(false);
        if (cycle != null) StopCoroutine(cycle);

        s1Manager.CleanUp();
        s2Manager.CleanUp();
        s3Manager.CleanUp();

        Destroy(npcCtl.gameObject);
        Destroy(playerCtl.gameObject);

        Destroy(s1Manager.gameObject);
        Destroy(s2Manager.gameObject);
        Destroy(s3Manager.gameObject);

        Bullet[] obj = GameObject.FindObjectsOfType<Bullet>();
        int c = obj.Length;
        for (int i = c - 1; i >= 0; i--)
        {
            Destroy(obj[i].gameObject);
        }

        Destroy(gameObject);
        yield break;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class FPSMB : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt;
    float totalDeltaTime = 0f;
    uint frameCount = 0;

    private void Start()
    {
        totalDeltaTime = 0f;
        frameCount = 0;
        StartCoroutine(FPSMeter());
    }

    IEnumerator FPSMeter()
    {
        yield return new WaitForFixedUpdate();
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (StageManager.isTesting)
            {
                BenchMB.Store(StageManager.spellPhase, totalDeltaTime, System.GC.GetTotalMemory(false));
            }
            txt.text = (frameCount / totalDeltaTime).ToString();
            totalDeltaTime = 0f;
            frameCount = 0;
        }
    }

    private void Update()
    {
        totalDeltaTime += Time.deltaTime;
        frameCount++;
    }
}

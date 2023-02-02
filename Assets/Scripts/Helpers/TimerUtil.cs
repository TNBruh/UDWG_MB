using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUtil : MonoBehaviour
{
    int cyclePassed = 0;
    float maxTime;
    float currentTime = 0;
    bool stackPassedCycle = false;

    public void Init(float maxTime, bool stack = false, bool enable = false)
    {
        this.maxTime = maxTime;
        this.stackPassedCycle = stack;
        gameObject.SetActive(enable);
    }

    private void OnEnable()
    {
        cyclePassed = 0;
        currentTime = 0;
    }

    public bool ConsumeCycle()
    {
        if (cyclePassed > 0)
        {
            cyclePassed--;
            return true;
        }
        return false;
    }

    private void Update()
    {
        bool addTime = !(!stackPassedCycle && cyclePassed > 0);
        currentTime += Time.deltaTime * (addTime ? 1 : 0);
        if (maxTime < currentTime)
        {
            if (addTime)
            {
                cyclePassed++;
            }

            float remain = currentTime - maxTime;
            currentTime = remain;
        }
    }

    public IEnumerator Switch(bool isActive)
    {
        //if (!isActive) GameCtl.Print($"turning off timer util");
        cyclePassed = 0;
        currentTime = 0;
        gameObject.SetActive(isActive);
        yield return null;
    }
}

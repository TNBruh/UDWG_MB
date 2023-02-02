using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCtl : MonoBehaviour
{
    [SerializeField] internal Button continueBtn;
    [SerializeField] internal Button exitBtn;

    internal void Init(UnityEngine.Events.UnityAction exitFunc)
    {
        exitBtn.onClick.AddListener(exitFunc);
    }

    private void Start()
    {
        continueBtn.onClick.AddListener(Continue);
    }

    internal void Continue()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}

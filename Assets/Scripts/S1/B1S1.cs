using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1S1 : Bullet
{
    [SerializeField] GameObject bouncedPrefab;

    static internal BS1Ctl castedCtl;
    protected override void Start()
    {
        castedCtl.Add(this);
        StartCoroutine(Bounce());
    }

    IEnumerator Bounce()
    {
        yield return new WaitWhile(() => TransformUtil.IsInBarrier(transform.position));
        Instantiate(bouncedPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, 180));
        castedCtl.Remove(this, true);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        castedCtl.Remove(this, true);
    }
}

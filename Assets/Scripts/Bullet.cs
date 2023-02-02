using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    internal static BulletCtl bulletCtl;

    protected abstract void Start();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        bulletCtl.Remove(this, true);
    }
}

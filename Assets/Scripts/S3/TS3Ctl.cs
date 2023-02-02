using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class TS3Ctl : BulletCtl
{
    internal float prog = 0;
    [SerializeField]
    internal Vector2[] introPositions = new Vector2[2]
    {
        new Vector2(0,6),
        new Vector2(0, 4),
    };
    [SerializeField]
    internal GameObject tPrefab;
    TS3 t;

    public bool FinishedIntro
    {
        get
        {
            return prog >= 1;
        }
    }

    private void Start()
    {
        TS3.castedCtl = this;
        TS3.bulletCtl = this;

    }
    internal override void Update()
    {
        if (t != null)
        {
            prog = math.clamp(prog + speed * Time.deltaTime, 0, 1);
            t.transform.position = Vector3.Lerp(introPositions[0], introPositions[1], prog);
        }
    }

    internal void Spawn()
    {
        t = Instantiate(tPrefab, introPositions[0], Quaternion.identity).GetComponent(typeof(TS3)) as TS3;
    }

    internal override void Clear()
    {
        Destroy(t.gameObject);
    }

}

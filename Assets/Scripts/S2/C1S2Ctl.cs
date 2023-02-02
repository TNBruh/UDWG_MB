using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class C1S2Ctl : BulletCtl
{
    [SerializeField] GameObject c1Prefab;
    S2Manager s2Manager;
    private void Start()
    {
        s2Manager = GameObject.FindObjectOfType(typeof(S2Manager)) as S2Manager;
        C1S2.bulletCtl = this;
        C1S2.castedCtl = this;
    }
    internal override void Update()
    {
        MoveBullet();
    }

    internal void Spawn()
    {
        Vector3 dir = s2Manager.playerCtl.transform.position - s2Manager.npcCtl.transform.position;

        float rad = math.atan2(dir.y, dir.x) + math.radians(90);
        for (int i = 0; i < 6; i++)
        {
            Instantiate(c1Prefab, s2Manager.npcCtl.transform.position, Quaternion.Euler(0, 0, math.degrees(rad + i * math.PI / 3)));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;
using Unity.Collections;

public abstract class BulletCtl : MonoBehaviour
{
    internal List<Transform> bulletTfsList = new List<Transform>();
    internal List<Bullet> bulletList = new List<Bullet>();
    internal TransformAccessArray BulletAccessArray
    {
        get
        {
            return new TransformAccessArray(bulletTfsList.ToArray());
        }
    }
    [SerializeField] internal float speed;

    protected struct MoveBulletJob : IJobParallelForTransform
    {
        [ReadOnly]
        public float deltaTime;
        [ReadOnly]
        public float speed;

        void IJobParallelForTransform.Execute(int index, TransformAccess transform)
        {
            float rot = math.radians(transform.rotation.eulerAngles.z);

            Vector3 movement = TransformUtil.RotateVector(Vector3.up, rot);
            movement *= speed * deltaTime;

            transform.position += movement;
        }
    }

    internal virtual void Clear()
    {
        int c = bulletTfsList.Count;
        for (int i = c-1; i >= 0; i--)
        {
            Remove(bulletList[i], true);
        }
        bulletList.Clear();
        bulletTfsList.Clear();
    }

    internal virtual void Add(Bullet bullet)
    {
        bulletList.Add(bullet);
        bulletTfsList.Add(bullet.transform);
    }

    internal virtual void Remove(Bullet bullet, bool destroy = false)
    {
        bulletList.Remove(bullet);
        bulletTfsList.Remove(bullet.transform);
        if (destroy) Destroy(bullet.gameObject);
    }

    internal virtual void MoveBullet()
    {
        TransformAccessArray transformAccessArray = new TransformAccessArray(bulletTfsList.ToArray());

        //execute job
        MoveBulletJob moveBulletJob = new MoveBulletJob
        {
            deltaTime = Time.deltaTime,
            speed = speed,
        };

        moveBulletJob.Schedule(transformAccessArray).Complete();
        transformAccessArray.Dispose();
    }

    internal abstract void Update();
}

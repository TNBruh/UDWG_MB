using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using Random = Unity.Mathematics.Random;

public class PlayerCtl : MonoBehaviour
{
    static readonly float3 border = new float3
    {
        x = 3.45f,
        y = 4.65f,
    };
    static readonly float3 respawnPos = new float3
    {
        x = 0,
        y = -5.6f,
    };
    static readonly float3 entrancePos = new float3
    {
        x = 0,
        y = -3.5f,
    };
    internal float transitionSpeed = 1.2f, invulnCountdownSpeed = 0.8f, invulnTime = 4;
    static readonly float speed = 5.6f;
    static readonly float recoil = 0.06f;
    float revivalTransition = 0;
    bool isDead = true;
    float shiftPercentage = 0.36f;
    float recoilTimer = 0;
    //[SerializeField] GameObject timerPrefab;
    GameObject timerObj;
    [SerializeField] TimerUtil timer;
    [SerializeField] TimerUtil invulnTimer;
    //[SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform[] bulletSpawnerTfs;
    Random random = new Random();
    [SerializeField] PlayerBulletCtl playerBulletCtlPrefab;
    PlayerBulletCtl playerBulletCtl;
    [SerializeField] Collider2D cldr;
    


    private void Start()
    {
        //timerObj = Instantiate(timerPrefab, transform);
        //timer = timerObj.GetComponent<TimerUtil>();
        timer.Init(recoil, false, true);
        random.InitState();
        //ThreadLocal<float> threadLocal = new ThreadLocal<float>(() =>
        //{
        //    System.Random rand = new System.Random();
        //});
        playerBulletCtl = Instantiate(playerBulletCtlPrefab, this.transform).GetComponent(typeof(PlayerBulletCtl)) as PlayerBulletCtl;
    }


    private void Update()
    {
        if (!isDead && !StageManager.isTesting)
        {
            Vector3 movement = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical"),
            } * speed * (Input.GetButton("Shift") ? shiftPercentage : 1) * Time.deltaTime;

            transform.position += movement;
            transform.position = new Vector3 {
                x = math.clamp(transform.position.x, -border.x, border.x),
                y = math.clamp(transform.position.y, -border.y, border.y),
            };

            if (Input.GetButton("Fire1") && timer.ConsumeCycle()) //fire
            {
                for (int i = 0; i < bulletSpawnerTfs.Length; i++)
                {
                    //decided to single-thread this process of shot spread
                    //because there are only 4 required random numbers
                    //instantiating random class instances in 5 threads might be slower

                    Quaternion rot = Quaternion.Euler(0, 0, random.NextFloat(-20f, 20f));
                    playerBulletCtl.Spawn(bulletSpawnerTfs[i].position, rot);
                }
                playerBulletCtl.Spawn(transform.position, Quaternion.identity);
            }
        } else if (isDead)
        {
            revivalTransition = math.clamp(revivalTransition + transitionSpeed * Time.deltaTime, 0, 1);
            transform.position = Vector3.Lerp(respawnPos, entrancePos, revivalTransition);
            if (revivalTransition == 1)
            {
                isDead = false;
                revivalTransition = 0;
                if (StageManager.isTesting) transform.position = new Vector3(0, 20, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isDead = true;
        if (cldr.enabled) StartCoroutine(Invulnerability());
    }

    IEnumerator Invulnerability()
    {
        cldr.enabled = false;
        invulnTimer.Init(invulnTime, false, true);
        yield return new WaitUntil(() => invulnTimer.ConsumeCycle());
        invulnTimer.Switch(false);
        cldr.enabled = true;
    }
}

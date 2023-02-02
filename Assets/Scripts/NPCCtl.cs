using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

public class NPCCtl : MonoBehaviour
{
    [SerializeField] Vector3[] positions = new Vector3[2];
    [SerializeField] Image healthtorus;
    [SerializeField] Collider2D cldr;
    [SerializeField] float maxHealth = 1200;
    internal float health = 1200;
    public float speed = 1.2f;
    float prog = 0;
    bool isRegenerating = false;
    public bool IsOnDestination
    {
        get
        {
            return transform.position == positions[1];
        }
    }
    public bool IsMaxHealth
    {
        get
        {
            return health >= maxHealth;
        }
    }

    private void Start()
    {
        health = maxHealth;
        healthtorus = GameObject.FindGameObjectWithTag("HealthRing").GetComponent(typeof(Image)) as Image;
        healthtorus.fillAmount = health / maxHealth;
    }

    //internal virtual bool LerpMovement(float3 initialPos, float3 endPos, float prog)
    //{
    //    transform.position = math.lerp(initialPos, endPos, prog);
    //    if (prog == 1) return true;

    //    return false;
    //}

    internal virtual void SetMovement(Vector3 endPos, float? newSpeed = null)
    {
        speed = (float)(newSpeed.HasValue ? newSpeed : speed);
        positions[0] = transform.position;
        positions[1] = endPos;
        prog = 0;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(positions[0], positions[1], prog);
        prog = math.clamp(prog + speed * Time.deltaTime, 0, 1);

        if (isRegenerating)
        {
            health = math.clamp(health+14, 0, maxHealth);
            if (IsMaxHealth)
            {
                isRegenerating = false;
            }
        }

        healthtorus.transform.position = transform.position;
        healthtorus.fillAmount = health / maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        health--;
    }

    internal void Regenerate()
    {
        cldr.enabled = false;
        isRegenerating = true;
    }

    internal void SetInvuln(bool invuln)
    {
        cldr.enabled = !invuln;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    //stats
    [SerializeField] protected float maxSpeed;
    protected float speed;
    public Stat health;
    public Stat armour;
    public RegenStat shield;
    [HideInInspector] public Vector3 movement, aimDir;

    //effects
    [SerializeField] GameObject ShieldEffect;
    [SerializeField] GameObject damageEffect;
    [SerializeField] GameObject killEffect;
    [SerializeField] float effectScale, effectHeight;
    [SerializeField] bool dontKill;

    //misc
    [HideInInspector] public new Rigidbody rigidbody;
    [HideInInspector] public int id;
    [HideInInspector] public HitData lastHit;

    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        health.Max();
        armour.Max();
        shield.Max();
    }

    protected virtual void Update()
    {
        shield.Regen();
        if (Time.timeScale == 1)
        {
            transform.LookAt(transform.position + aimDir);
        }
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(movement * speed);
    }

    public void Damage(float damage, Vector3 force, Vector3 pos, HitData hitData)
    {
        if (shield.value >= damage)
        {
            shield.value -= damage;
        }
        else if (armour.value >= (damage - shield.value))
        {
            armour.value -= (damage - shield.value);
            shield.value = 0f;
        }
        else
        {
            Instantiate(damageEffect, pos, force == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(-force));
            health.value -= (damage - shield.value - armour.value);
            shield.value = 0f;
            armour.value = 0f;
        }

        lastHit = hitData;
        if (health.value <= 0f)
        {
            lastHit.damage = damage + health.value;
            health.value = 0f;
            Kill();
        }
        else
        {
            hitData.damage = damage;
            rigidbody.AddForce(force);
        }
        if (lastHit.weapon != null)
        {
            Tracking.Log(lastHit);
        }
    }

    protected virtual void Kill()
    {
        Rigidbody effect = Instantiate(killEffect, transform.position, transform.rotation).GetComponent<Rigidbody>();
        effect.transform.localScale *= effectScale;
        effect.transform.position += transform.up * effectHeight;
        if (!dontKill)
        {
            Destroy(gameObject, 0.05f);
        }
    }
}

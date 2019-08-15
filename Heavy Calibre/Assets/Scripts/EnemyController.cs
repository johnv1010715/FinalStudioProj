using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Agent
{
    [SerializeField] float damage, force, maxAttackCooldown;
    float attackCooldown;
    [SerializeField] Weapon weapon;

    PlayerController player;

    [SerializeField] float maxStuckTime, minStuckDistance, unStickForce;
    float stuckTimer;
    Vector3 lastPos;

    [SerializeField] int points;
    [SerializeField] GameObject drop;
    bool kill;

    protected override void Start()
    {
        base.Start();
        id = Random.Range(1000, 10000);
        player = GameController.players[0];
        speed = maxSpeed;
        if (weapon)
        {
            weapon.agent = this;
        }
    }

    protected override void Update()
    {
        base.Update();
        attackCooldown -= Time.deltaTime;
        if (player.isActiveAndEnabled)
        {
            movement = (player.transform.position - transform.position).normalized;
            if(attackCooldown < 0)
            {

            }
        }

        if ((lastPos - transform.position).magnitude <= minStuckDistance)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= maxStuckTime)
            {
                rigidbody.AddForce(Vector3.up * unStickForce);
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }
        lastPos = transform.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (weapon)
            {
                Kill();
            }
            else if(attackCooldown <= 0)
            {
                collision.collider.GetComponent<Agent>().Damage(damage, (player.transform.position - transform.position).normalized * force, collision.GetContact(0).point, new HitData());
                attackCooldown = maxAttackCooldown;
            }
        }
    }

    protected override void Kill()
    {
        if (!kill)
        {
            kill = true;
            if (weapon)
            {
                aimDir = Vector3.down;
                weapon.TriggerDown();
            }
            if (!GameController.dontScore)
            {
                FindObjectOfType<GameController>().AddScore(points);
                if (drop)
                {
                    Instantiate(drop, transform.position, Quaternion.identity);
                }
                if (lastHit.weapon != null)
                {
                    Tracking.Log(new KillData(gameObject.name, lastHit.weapon));
                }
            }
            base.Kill();
        }
    }
}

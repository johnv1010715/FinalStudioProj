using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Agent
{
    [SerializeField] float damage, force, maxAttackCooldown;
    float attackCooldown;
    [SerializeField] protected Weapon weapon;

    protected PlayerController player;

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
        attackCooldown -= Time.deltaTime;
        movement = (player.transform.position - transform.position).normalized;
        aimDir = movement.normalized;
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
                collision.collider.GetComponent<Agent>().Damage(damage, Vector3.zero * force, collision.GetContact(0).point, new HitData());
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

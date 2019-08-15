using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float damage, force, radius;
    [SerializeField] int penetration;
    [HideInInspector] public int health;

    [SerializeField] Type type;
    
    [HideInInspector] public HitData hitData;

    [SerializeField] GameObject effect;
    [SerializeField] ParticleSystem[] particleSystems;
    [SerializeField] float particlePersist;

    bool kill = false;

    void Start()
    {
        health = penetration;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!kill)
        {
            var otherAgent = other.GetComponent<Agent>();
            if (!(otherAgent && otherAgent.id == hitData.id))
            {
                if (type != Type.explosive)
                {
                    Vector3 dir = GetComponent<Rigidbody>().velocity.normalized;
                    Vector3 pos = other.ClosestPointOnBounds(transform.position);
                    RaycastHit hit;
                    if (otherAgent)
                    {
                        otherAgent.Damage(damage, dir * force, pos, hitData);
                        Kill();
                    }
                    else if (Physics.Raycast(pos - dir, dir, out hit))
                    {
                        Object col = hit.collider.gameObject.GetComponentInParent<Object>();
                        if (!col || col.penetrationCost >= health)
                        {
                            Kill();
                        }
                        else
                        {
                            health -= col.penetrationCost;
                            Instantiate(effect, pos, Quaternion.LookRotation(Vector3.Reflect(-dir, hit.normal)));
                        }
                        Instantiate(effect, pos, Quaternion.LookRotation(Vector3.Reflect(dir, hit.normal)));
                    }
                }
                else
                {
                    foreach (Collider collider in Physics.OverlapSphere(transform.position, radius))
                    {
                        var agent = collider.GetComponent<Agent>();
                        var weapon = collider.GetComponent<Weapon>();
                        if (agent || weapon)
                        {
                            Vector3 dir = collider.ClosestPointOnBounds(transform.position) - transform.position;
                            float falloff = 1 - (Vector3.Magnitude(dir) / radius);
                            if (agent)
                            {
                                agent.Damage(damage * falloff * (agent.id == hitData.id ? 0.5f : 1f), dir.normalized * force * falloff, collider.ClosestPointOnBounds(transform.position), hitData);
                            }
                            else if (weapon)
                            {
                                weapon.GetComponent<Rigidbody>().AddForce(force * (dir + (Vector3.up * 5)).normalized * falloff);
                            }
                        }
                    }
                    Instantiate(effect, transform.position, Quaternion.identity);
                    Kill();
                }
            }
        }
    }

    void Kill()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        foreach (ParticleSystem ps in particleSystems)
        {
            var em = ps.emission;
            em.enabled = true;
            em.rateOverTime = 0f;
        }
        Destroy(gameObject, particlePersist);
        kill = true;
    }

    enum Type {bullet, explosive}
}
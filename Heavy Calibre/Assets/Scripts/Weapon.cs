using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform[] pSpawn;
    public GameObject effect;
    public float effectDelay;
    public Transform eSpawn;
    [HideInInspector] public bool equiped = false;
    [HideInInspector] public int equipmentIndex;
    [HideInInspector] public Transform targetTransform;
    [HideInInspector] public Agent agent;

    public Weapon other; //used for duel-wielding

    [SerializeField] bool ignorObstruction;

    Animator animator;

    float accuracy, stability, cooldown, warmup;
    int burstIndex, fireMode, capacity;
    bool triggerDown;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        cooldown -= Time.deltaTime;
        if (equiped && targetTransform)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * 5 * data.mobility);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 5 * data.mobility);
        }
        if (cooldown <= 0)
        {
            if (burstIndex > 0)
            {
                burstIndex--;
                Shoot();
            }
            else if (data.burstCount[fireMode] == 0 && triggerDown)
            {
                Shoot();
            }
        }

        if (Time.deltaTime > 0)
        {
            accuracy = Mathf.MoveTowardsAngle(accuracy, data.minAccuracy, (data.maxAccuracy - data.minAccuracy) * (Time.deltaTime / data.recoveryTime));
            stability = Mathf.MoveTowards(stability, triggerDown && warmup == data.maxWarmup ? data.maxStability : 0, Time.deltaTime / data.stabilityTime);
            warmup = Mathf.MoveTowards(warmup, triggerDown ? data.maxWarmup : 0, Time.deltaTime);
        }
    }

    public void TriggerDown()
    {
        if (!triggerDown)
        {
            if (data.burstCount[fireMode] > 0)
            {
                burstIndex = data.burstCount[fireMode];
            }
            triggerDown = true;
        }
        if(other)
        {
            other.TriggerDown();
        }
    }

    public void TriggerUp()
    {
        if (triggerDown)
        {
            if (burstIndex <= 1)
            {
                burstIndex = 0;
            }
            triggerDown = false;
        }
        if(other)
        {
            other.TriggerUp();
        }
    }

    public void ToggleFireMode()
    {
        if (fireMode < data.burstCount.Length - 1)
        {
            fireMode++;
        }
        else
        {
            fireMode = 0;
        }
        if(other)
        {
            other.ToggleFireMode();
        }
    }

    void Shoot()
    {
        if(pSpawn.Length > 1)
        {
            for(int i = 0; i < pSpawn.Length; i++)
            {
                Shoot(i);
            }
        }
        else
        {
            Shoot(0);
        }
    }

    void Shoot(int barrelIndex)
    {
        if (ignorObstruction || !Physics.CheckSphere(pSpawn[barrelIndex].position, 0.25f, ~(1<<10)))
        {
            if (warmup == data.maxWarmup)
            {
                Vector3 dir = Quaternion.AngleAxis(Random.Range(-accuracy, accuracy), Vector3.up) * (agent ? agent.aimDir : transform.rotation * Vector3.forward);
                Vector3 aimPos = agent ? agent.transform.position + (dir * 10f) : pSpawn[barrelIndex].position + dir;
                aimPos.y = transform.position.y;
                Projectile projectile = Instantiate(data.projectile, pSpawn[barrelIndex].position, Quaternion.identity).GetComponent<Projectile>();
                projectile.transform.LookAt(aimPos);
                projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * data.initialVelocity;
                projectile.hitData = new HitData((agent && !ignorObstruction) ? agent.id : 100, gameObject.name);
                if (agent)
                {
                    agent.rigidbody.AddForce(dir * -data.recoil * (1 - stability));
                }
                if (effect)
                {
                    Invoke("DoEffect", effectDelay);
                }
            }
            if (animator)
            {
                animator.SetBool("Trigger", true);
            }
            accuracy = Mathf.LerpAngle(accuracy, data.maxAccuracy, data.spreadDelta);
            cooldown = data.maxCooldown[fireMode];
        }
    }
    
    public void Pickup(PlayerController player, int index, bool switchTo = false)
    {
        equiped = true;
        accuracy = data.maxAccuracy;
        stability = 0;
        if (player.equipment.IsEmpty())
        {
            player.equipment.weapons[0] = this;
            equipmentIndex = 0;
            switchTo = true;
        }
        else
        {
            if (player.equipment.weapons[index])
            {
                player.equipment.weapons[index].other = this;
            }
            else
            {
                player.equipment.weapons[index] = this;
            }
            equipmentIndex = index;
        }
        transform.parent = player.transform;
        agent = player;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        player.SelectWeapon(switchTo ? equipmentIndex : player.selectedWeapon);
    }

    public void Drop(PlayerController player, float force, bool dropAll)
    {
        if (other)
        {
            other.Drop(force);
            other = null;
            if(dropAll)
            {
                player.equipment.weapons[equipmentIndex] = null;
                Drop(force);
            }
        }
        else
        {
            player.equipment.weapons[equipmentIndex] = null;
            Drop(force);
        }
    }

    public void SwapHands(PlayerController player)
    {
        if (data.oneHanded)
        {
            if (other)
            {
                player.equipment.weapons[equipmentIndex] = other;
                other.equipmentIndex = equipmentIndex;
                other.other = this;
                other = null;
                player.SelectWeapon(player.selectedWeapon);
            }
            else if (player.equipment.FindIndex(false, data.oneHanded, out int index, equipmentIndex, true))
            {
                if (player.equipment.weapons[index])
                {
                    if (player.equipment.weapons[index].other)
                    {
                        other = player.equipment.weapons[index].other;
                        player.equipment.weapons[index].other = null;
                    }
                    else
                    {
                        other = player.equipment.weapons[index];
                        player.equipment.weapons[index] = null;
                    }
                    player.SelectWeapon(player.selectedWeapon);
                }
                else
                {
                    player.SwapWeapon(equipmentIndex, index);
                }
            }
        }
    }

    public void Drop(float force)
    {
        equiped = false;
        transform.parent = null;
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(agent.aimDir * force);
        rigidbody.AddTorque(new Vector3(200, 500, 200));
        TriggerUp();
    }

    public bool AtTarget()
    {
        return ((targetTransform.position - transform.position).magnitude <= 0.25f);
    }

    public void DoEffect()
    {
        Instantiate(effect, eSpawn.transform).transform.parent = null;
    }
}
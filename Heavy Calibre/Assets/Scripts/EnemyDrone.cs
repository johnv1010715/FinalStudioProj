using UnityEngine;
using System.Collections;

public class EnemyDrone : EnemyController
{
    [SerializeField] float maxStuckTime, minStuckDistance, unStickForce;
    float stuckTimer;
    Vector3 lastPos;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Update()
    {
        base.Update();
        if (player.isActiveAndEnabled)
        {
            transform.LookAt(transform.position + aimDir);
            if((player.transform.position - transform.position).magnitude < 10f)
            {
                weapon.TriggerDown();
            }
            else
            {
                weapon.TriggerUp();
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
}

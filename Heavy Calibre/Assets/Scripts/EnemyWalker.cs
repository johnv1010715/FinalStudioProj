using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyWalker : EnemyController
{
    NavMeshAgent meshAgent;

    protected override void Start()
    {
        base.Start();
        meshAgent = GetComponent<NavMeshAgent>();
        weapon.TriggerDown();
    }

    protected override void FixedUpdate()
    {

    }

    protected override void Update()
    {
        base.Update();
        meshAgent.SetDestination(GameController.players[0].transform.position);
    }
}

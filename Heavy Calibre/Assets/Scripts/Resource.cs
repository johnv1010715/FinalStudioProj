using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] float minDistance;
    new Rigidbody rigidbody;
    bool kill;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddTorque(new Vector3(100, 500, 300));
    }
    
    private void FixedUpdate()
    {
        if (!kill)
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(GameController.resourcesIcon.rectTransform.position + new Vector3(0, 0, 10));
            var dir = targetPos - transform.position;
            if (dir.magnitude > minDistance)
            {
                rigidbody.AddForce((dir * 5) + (dir.normalized * 50));

            }
            else
            {
                Kill();
            }
        }
    }

    void Kill()
    {
        kill = true;
        FindObjectOfType<GameController>().AddResource();
        Destroy(gameObject);
    }
}
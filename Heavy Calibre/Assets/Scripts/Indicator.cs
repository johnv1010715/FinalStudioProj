using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Transform targetTransform;

    private void Start()
    {
        transform.parent = Minimap.map.transform;
    }

    void Update()
    {
        if (targetTransform)
        {
            transform.localPosition = Minimap.WorldToMiniMapPos(targetTransform.position);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

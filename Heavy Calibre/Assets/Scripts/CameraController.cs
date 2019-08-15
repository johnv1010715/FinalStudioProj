using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 offset;
    Quaternion rot;

    void Start()
    {
        offset = transform.position;
        rot = transform.rotation;
    }
    
    void Update()
    {
        transform.position = GameController.players[0].transform.position + offset;
    }

    public void ResetRot()
    {
        transform.rotation = rot;
    }
}

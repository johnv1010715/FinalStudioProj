using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float maxKillTimer;
    [SerializeField] bool fadeLightSource;

    Light lightSource;

    float maxIntensity;

    void Start()
    {
        if (fadeLightSource)
        {
            lightSource = GetComponent<Light>();
            maxIntensity = lightSource.intensity;
        }
        Destroy(gameObject, maxKillTimer);
    }

    private void Update()
    {
        if (fadeLightSource)
        {
            lightSource.intensity -= (maxIntensity * Time.deltaTime) / maxKillTimer;
        }
    }
}
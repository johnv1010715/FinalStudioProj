using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat
{
    [SerializeField] public float maxValue;
    [HideInInspector] public float value;

    public void Max()
    {
        value = maxValue;
    }

    public void Add(float value)
    {
        this.value = Mathf.Min(this.value + value, maxValue);
    }

    public void SetScale(RectTransform rect)
    {
        rect.localScale = maxValue == 0 ? Vector3.zero : new Vector3(value / maxValue, 1, 1);
    }
}

[System.Serializable]
public class RegenStat : Stat
{
    public float rate;

    public void Regen()
    {
        if (value < maxValue)
        {
            Add(rate * Time.deltaTime);
        }
    }
}
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WeaponData
{
    public GameObject projectile;
    public float initialVelocity;
    public float recoil;
    [Range(0f, 90f)] public float minAccuracy, maxAccuracy;
    [Range(0f, 1f)] public float spreadDelta, maxStability;
    public float recoveryTime, stabilityTime;
    public int[] burstCount; //0 = auto, 1 = single, 2+ = burst
    public float[] maxCooldown; //for each firemode
    public float maxWarmup;
    public int maxCapacity, weight;
    public float mobility;
    public bool oneHanded;
}
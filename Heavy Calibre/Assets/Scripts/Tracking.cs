using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tracking : MonoBehaviour
{
    public Transform objectiveList;
    public GameObject listElement;
    public List<Objective> missions = new List<Objective>();
    [Header("Random elements")]
    public string[] enemies;
    public string[] weapons;

    public static Stats stats = new Stats();
    public static List<Objective> objectives = new List<Objective>();

    void Start()
    {
        objectives = missions;
        DisplayObjectives(listElement,objectiveList);
    }

    public void AddRandomObjectives(int count)
    {
        for(int i = 0; i < count; i++)
        {
            objectives.Add(Objective.GetRandom(enemies, weapons));
        }
        DisplayObjectives(listElement, objectiveList);
    }

    public static void DisplayObjectives(GameObject listElement, Transform objectiveList)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.display == null)
            {
                objective.progress.maxValue = objective.count;
                objective.display = Instantiate(listElement, objectiveList).GetComponent<UIObjective>();
                objective.description = "Kill " + objective.progress.maxValue + (objective.killData.name == "" ? " enemie" : " " + objective.killData.name) + "s" + (objective.killData.weapon == "" ? "" : " using the " + objective.killData.weapon);
                objective.display.description.text = objective.description;
            }
        }
    }

    public static void Log(KillData killData)
    {
        killData.name = killData.name.Replace("(Clone)", "");
        killData.weapon = killData.weapon.Replace("(Clone)", "");
        List<Objective> completed = new List<Objective>();
        foreach (Objective objective in objectives)
        {
            if (!objective.complete && (objective.killData.name == "" || objective.killData.name == killData.name) && (objective.killData.weapon == "" || objective.killData.weapon == killData.weapon))
            {
                objective.progress.Add(1);
                objective.DisplayProgress();

                if (objective.progress.value == objective.progress.maxValue)
                {
                    Debug.Log("Objective complete! '" + objective.display.description.text + "'");
                    objective.complete = true;
                    objective.display.GetComponent<Animator>().SetBool("Trigger",true);
                    FindObjectOfType<GameController>().AddTokens(objective.reward);
                    Destroy(objective.display.gameObject, 1f);
                    completed.Add(objective);
                }
            }
        }
        foreach (Objective objective in completed)
        {
            objectives.Remove(objective);
        }

        int value;

        if (stats.weaponKills.TryGetValue(killData.weapon, out value))
        {
            stats.weaponKills.Remove(killData.weapon);
            stats.weaponKills.Add(killData.weapon, value + 1);
        }
        else
        {
            stats.weaponKills.Add(killData.weapon, 1);
        }

        if (stats.enemyKills.TryGetValue(killData.name, out value))
        {
            stats.enemyKills.Remove(killData.name);
            stats.enemyKills.Add(killData.name, value + 1);
        }
        else
        {
            stats.enemyKills.Add(killData.name, 1);
        }
        stats.kills++;
    }

    public static void Log(HitData hitData)
    {
        hitData.weapon = hitData.weapon.Replace("(Clone)", "");
        if (stats.weaponDamage.TryGetValue(hitData.weapon, out float value))
        {
            stats.weaponDamage.Remove(hitData.weapon);
            stats.weaponDamage.Add(hitData.weapon, value + hitData.damage);
        }
        else
        {
            stats.weaponDamage.Add(hitData.weapon, hitData.damage);
        }
        stats.damageDealt++;
    }
}

public class Stats
{
    public Dictionary<string, int> enemyKills = new Dictionary<string, int>();
    public Dictionary<string, int> weaponKills = new Dictionary<string, int>();
    public Dictionary<string, float> weaponDamage = new Dictionary<string, float>();
    public int kills, deaths, resourcesGathered, resourcesSpent;
    public float damageTaken, damageDealt, distanceTraveled;
}

[System.Serializable]
public class Objective
{
    public KillData killData;
    public int count;
    [HideInInspector] public string description;
    [HideInInspector] public Stat progress;
    [HideInInspector] public UIObjective display;
    public int reward;
    [HideInInspector] public bool complete;

    public static Objective GetRandom(string[] enemies, string[] weapons)
    {
        Objective randomObjective = new Objective();
        randomObjective.killData = new KillData(enemies[Random.Range(0, enemies.Length)], weapons[Random.Range(0, weapons.Length)]);
        randomObjective.count = Random.Range(1, 11) * 10;
        randomObjective.progress = new Stat();
        randomObjective.reward = randomObjective.count / 2;
        return randomObjective;
    }

    public void DisplayProgress()
    {
        progress.SetScale(display.progressBar);
    }
}

[System.Serializable]
public struct KillData
{
    public string name, weapon;

    public KillData(string Name, string Weapon)
    {
        name = Name;
        weapon = Weapon;
    }
}

public struct HitData
{
    public int id;
    public string weapon;
    public float damage;

    public HitData(int Id, string Weapon, float Damage = 0f)
    {
        id = Id;
        weapon = Weapon;
        damage = Damage;
    }
}

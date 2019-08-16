using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    public int playerCount;

    public GameObject[] weapons;

    public WeaponSchematic[] weaponSchematics;
    public ShieldSchematic[] shieldSchematics;
    public ArmourSchematic[] armourSchematics;

    public GameObject spawnEffect;

    [SerializeField] SpawnData[] enemies;
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] float spawnRadius;
    
    [SerializeField] float waveBaseTime, waveCooldown;
    [HideInInspector] public static bool waveStarted, waveCleared, gameover, dontScore;
    WaveData currentWave;
    int waveIndex;
    float waveTime;
    float waveTimer;
    int difficulty;

    [SerializeField] float baseSize;

    [SerializeField] Text waveText;

    public static int score;
    [SerializeField] Text scoreText;

    public static int resources;
    [SerializeField] Text resourceText;
    [SerializeField] Image setResourcesIcon;
    public static Image resourcesIcon;

    public static int tokens;
    [SerializeField] Text tokensText;
    [SerializeField] Image tokensIcon;

    public static PlayerController[] players;

    MenuController menuController;

    public bool inMenu;

    void Start()
    {
        if (!inMenu)
        {
            menuController = FindObjectOfType<MenuController>();
            resourcesIcon = setResourcesIcon;
            players = new PlayerController[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                players[i] = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
            }
            SpawnWeapon(3, 0, false);
            menuController.DisplayWeapons(weaponSchematics);
            menuController.DisplayEquipment(armourSchematics, shieldSchematics);
            menuController.SetMenu("Start");
            menuController.playerHUD = players[0].GetComponentInChildren<Canvas>();
        }
    }
    
    void Update()
    {
        if (!inMenu)
        {
            if (!gameover)
            {
                if (waveStarted)
                {
                    if (waveTimer > 0)
                    {
                        float waveProgress = 1 - waveTimer / waveTime;
                        for (int i = 0; i < currentWave.spawnPool.Length; i++)
                        {
                            if (currentWave.spawnPool[i].enemiesSpawned >= currentWave.spawnPool[i].count || waveProgress < currentWave.spawnPool[i].startDistribution || waveProgress >= currentWave.spawnPool[i].endDistribution) continue;

                            if (currentWave.spawnPool[i].spawnTimer <= 0)
                            {
                                SpawnEnemy(currentWave.spawnPool[i]);

                                float newOffset = Random.Range(0, 1f);
                                currentWave.spawnPool[i].spawnTimer = (currentWave.spawnPool[i].maxSpawnTimer * (1 - currentWave.spawnPool[i].timerOffset)) + (currentWave.spawnPool[i].maxSpawnTimer * newOffset);
                                currentWave.spawnPool[i].timerOffset = newOffset;
                                currentWave.spawnPool[i].enemiesSpawned++;
                            }
                            else
                            {
                                currentWave.spawnPool[i].spawnTimer -= Time.deltaTime;
                            }
                        }
                        waveTimer -= Time.deltaTime;
                    }
                    else
                    {
                        EndWave();
                    }
                }
                else if (waveIndex > 0)
                {
                    if (waveTimer > 0)
                    {
                        waveText.text = "NEXT WAVE: " + " " + waveTimer.ToString("0.0");
                        waveTimer -= Time.deltaTime;
                        if (!waveCleared && FindObjectsOfType<EnemyController>().Length == 0)
                        {
                            waveCleared = true;
                            menuController.SetMenu("NextWave", true, true, false);
                        }
                    }
                    else
                    {
                        StartWave();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseUnpause();
            }

            if (players[0].IsInsideBase(baseSize))
            {
                if (Input.GetButtonDown("Buy"))
                {
                    menuController.SetMenu("Buy");
                }
            }
            else if (menuController.GetMenu() == "Buy")
            {
                menuController.SetMenu("Buy");
            }
        }
    }

    public void StartGame()
    {
        gameover = false;
        StartWave(1);
    }

    public void StartWave(int setWave)
    {
        waveIndex = setWave;
        StartWave();
    }

    public void StartWave()
    {
        difficulty = 2 + (waveIndex * 2);
        waveTime = waveBaseTime + difficulty;
        List<SpawnData> spawnRange = new List<SpawnData>();
        SpawnData[] newSpawnPool = new SpawnData[difficulty];
        foreach (SpawnData enemy in enemies)
        {
            if (enemy.minDifficulty <= difficulty && (enemy.maxDifficulty == 0 || enemy.maxDifficulty >= difficulty))
            {
                spawnRange.Add(enemy);
            }
        }
        for (int i = 0; i < difficulty; i++)
        {
            newSpawnPool[i] = spawnRange[Random.Range(0, spawnRange.Count)];
        }
        for (int i = 0; i < newSpawnPool.Length; i++) //initialize spawn sets
        {
            newSpawnPool[i].maxSpawnTimer = (waveTime / newSpawnPool[i].count) * (1 / (newSpawnPool[i].endDistribution - newSpawnPool[i].startDistribution));
            newSpawnPool[i].timerOffset = 1;
        }
        currentWave = new WaveData { spawnPool = newSpawnPool};
        waveStarted = true;
        waveCleared = false;
        waveTimer = waveTime;
        waveText.text = "WAVE " + waveIndex;
        menuController.SetMenu(null, true, true, false);
    }

    public void EndWave()
    {
        currentWave = null;
        waveStarted = false;
        if (!gameover)
        {
            //KillAll(false, false);
            waveIndex++;
            waveTimer = waveCooldown;
        }
        else
        {
            waveTimer = 0;
            waveText.text = "WAVE " + waveIndex;
        }
    }

    public void EndGame()
    {
        gameover = true;
        EndWave();
        menuController.SetMenu("Gameover", true, true, false);
    }

    public void ResetGame()
    {
        gameover = true;
        EndWave();
        AddScore(-score);
        AddResource(-resources);
        KillAll();
        players[0].Respawn();
        SpawnWeapon(3, 0);
        waveText.text = null;
        menuController.DisplayWeapons(weaponSchematics);
        menuController.DisplayEquipment(armourSchematics, shieldSchematics);
        menuController.SetMenu("Start",true, true, false);
        menuController.EnableHUD(true);
        FindObjectOfType<Tracking>().AddRandomObjectives(Tracking.objectives.Count > 0 ? 3 - Tracking.objectives.Count : 4);
        Time.timeScale = 1;
    }

    public void KillAll(bool killResource = true, bool killWeapons = true, bool killEnemies = true)
    {
        if(killResource)
        {
            foreach (Resource resource in FindObjectsOfType<Resource>())
            {
                Destroy(resource.gameObject);
            }
        }
        if (killWeapons)
        {
            foreach (Weapon weapon in FindObjectsOfType<Weapon>())
            {
                Destroy(weapon.gameObject);
            }
        }
        if (killEnemies)
        {
            dontScore = true;
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.Damage(1000, Vector3.zero, enemy.transform.position, new HitData());
            }
            dontScore = false;
        }
    }

    public void SpawnWeapon(int id, int player, bool pickup = true)
    {
        SpawnWeapon(id, players[player], pickup);
    }

    public void SpawnWeapon(int id, PlayerController player, bool pickup = true)
    {
        int index;
        if (pickup)
        {
            Weapon weapon;
            SpawnWeapon(id, player.transform.position, out weapon);
            if (!player.equipment.FindIndex(weapons[id].GetComponent<Weapon>(), out index, player.selectedWeapon) || Input.GetButton("Modifier"))
            {
                index = player.selectedWeapon;
                player.DropWeapon(400f, !weapon.data.oneHanded);
            }
            weapon.Pickup(player, index);
            player.Purge(player.selectedWeapon);
        }
        else
        {
            SpawnWeapon(id, player.transform.position + new Vector3(0, 2, 2));
        }
    }

    public void SpawnWeapon(int id, Vector3 pos)
    {
        SpawnWeapon(id, pos, out Weapon weapon);
        weapon.equiped = false;
    }

    public void SpawnWeapon(int id, Vector3 pos, out Weapon weapon)
    {
        Instantiate(spawnEffect, pos, Quaternion.identity);
        weapon = Instantiate(weapons[id], pos, weapons[id].transform.rotation).GetComponent<Weapon>();
    }

    void SpawnEnemy(SpawnData enemy)
    {
        Transform spawn = spawnPositions[(int)enemy.spawnDirections[Random.Range(0, enemy.spawnDirections.Length)]];
        Vector3 spawnPos = spawn.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));
        Instantiate(enemy.enemy, spawnPos, spawn.rotation);
        //Debug.Log("Spawned " + Instantiate(enemy, spawnPos, Quaternion.identity).name + " at: " + (waveTimer > 60 ? ((int)(waveTimer / 60)).ToString("00") + ":" + ((int)(waveTimer % 60)).ToString("00") : waveTimer.ToString("00.00")));
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = "SCORE: " + score.ToString("00000");
    }

    public void AddResource(int value = 1)
    {
        resources += value;
        resourceText.text = "x " + resources.ToString("000");
        resourcesIcon.GetComponent<Animator>().SetBool("Trigger",true);
    }

    public void AddTokens(int value = 1)
    {
        tokens += value;
        tokensText.text = "x " + tokens.ToString("000");
        tokensIcon.GetComponent<Animator>().SetBool("Trigger", true);
    }

    public void PauseUnpause()
    {
        if (Time.timeScale == 1) //pause
        {
            Time.timeScale = 0;
            menuController.SetMenu("Pause");
            menuController.EnableHUD(false);
            players[0].ToggleInputs(false);
        }
        else //unpause
        {
            Time.timeScale = 1;
            menuController.Return(0,true);
            menuController.EnableHUD(true);
            players[0].ToggleInputs(true);
        }
    }

    public void ToggleAutoSwitch(bool toggle)
    {
        players[0].autoSwitch = toggle;
    }
}

[System.Serializable]
public class WaveData
{
    public SpawnData[] spawnPool;
}

[System.Serializable]
public struct SpawnData
{
    public string name;
    public GameObject enemy;
    public int count;
    public int minDifficulty, maxDifficulty;
    [Range(0, 1)] public float startDistribution, endDistribution;
    [HideInInspector] public float spawnRatio, maxSpawnTimer, spawnTimer, timerOffset;
    [HideInInspector] public int enemiesSpawned;

    public SpawnDirection[] spawnDirections;

    public enum SpawnDirection { North, East, South, West };
}

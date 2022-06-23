using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    
    //NavMesh
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public List<EnemyAI> spawnedUnits = new();

    // player stats

    private CashManager cashM;

    public TMP_Text goldText;
    public TMP_Text waveText;
    public TMP_Text killsText;
    public TMP_Text dpsText;
    public TMP_Text levelText;
    public TMP_Text timerText;

    public int wave = 1;
    public int kills = 0;
    public float dps = 0;
    public float sTimer;

    //Unit Spawn Resources
    public GameObject unitToSpawn;
    public float spawnCounter = 30.0f;
    public float baseSpawnCounter = 30.0f;
    public float sR = 10.0f;
    public int uSpawned = 0;
    public int totalSpawned=0;
    public int maxSpawned = 10;

    public float counter;
    public float health = 100;

    public void Awake()
    {
        cashM = GetComponent<CashManager>();
    }
    public void SpawnUnit(int _wave)
    {
        float healthMod = _wave + Mathf.Log(32 + _wave, 2);
        health = (1.0f + _wave) * Mathf.Floor(Mathf.Pow(1.01f, healthMod) + 1); 
        uSpawned += 1;
        totalSpawned += 1;
        var spawnLocation = new Vector3(Random.Range(-sR, sR), 2, Random.Range(-sR, sR));
        GameObject newSpawn = Instantiate(unitToSpawn, spawnLocation, Quaternion.identity);
        newSpawn.GetComponent<EnemyAI>().mName = "Monster " + uSpawned.ToString();
        newSpawn.GetComponent<EnemyAI>().maxHealth = 5 + health + wave;
    }
    public void TimerToSpawn()
    {
        if (totalSpawned >= maxSpawned) return;
        if (spawnCounter > 0.0f)
        {
            counter += Time.deltaTime;
            if (counter >= 0.1f)
            {
                spawnCounter -= counter;
                if (spawnCounter < 0)
                {
                    timerText.SetText("Time Till Spawn: " + 0.ToString("F3"));
                    counter = 0;
                }
                else
                {
                    timerText.SetText("Time Till Spawn: " + spawnCounter.ToString("F3"));
                    counter = 0;
                }
                    
            }
        }
        if (spawnCounter <= 0.0f)
        {
            SpawnUnit(wave);
            spawnCounter = baseSpawnCounter;
        }
    }
    public void IDied()
    {
        kills += 1;
        cashM.GrantKill(wave, 1);
        if (kills >= maxSpawned)
        {
            NextWave();
        }
    }
    public void NextWave()
    {
        wave += 1;
        totalSpawned = 0;
        uSpawned = 0;;
        player.transform.position = Vector3.zero;

        maxSpawned = (5 * wave) + (2 ^ wave);

        cashM.GrantKill(wave, 10);

    }
    public void FixedUpdate()
    {
        TimerToSpawn();
        if(Time.renderedFrameCount % 20 == 0)
       {
            SetText();
        }
    }
    public void SetText()
    {

        //global
        goldText.SetText("Gold: " + cashM.gold.ToString());

        //local
        waveText.SetText("Wave: " +wave.ToString());
        killsText.SetText("Kills: " +kills.ToString());
        dpsText.SetText("DPS: " +dps.ToString());
        levelText.SetText("Level: " +cashM.level.ToString());
    }
}

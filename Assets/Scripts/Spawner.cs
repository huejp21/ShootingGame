using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public enum UserLevel
    {
        beginer,
        medium,
        hard,
        count,
    }

    public bool devMode;

    public static UserLevel userLevel;

    public LevelOfDefficult[] levelOfDefficult = new LevelOfDefficult[(int)UserLevel.count];
    public Wave[] waves;
    public Wave[] orgWave { private set; get; }
    public Enemy enemy;

    public System.DateTime gameStartTime;
    bool enemySpwaned = true;
    LivingEntity playerEntity;
    Transform playerT;

    public Wave currentWave { private set; get; }
    int currentWaveNumber;

    public int enemyKilledCount { private set; get; }
    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    // Use this for initialization
    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisabled == false)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite)
            && (Time.time > nextSpawnTime)
            && enemySpwaned)
            {
                enemySpwaned = false;
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;//tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
        enemySpwaned = true;
    }


    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        enemyKilledCount++;
        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    
    void NextWave()
    {
        enemyKilledCount = 0;
        enemySpwaned = true;
        if (currentWaveNumber == 0)
        {
            SetUserLevel(userLevel);
            gameStartTime = System.DateTime.Now;
        }
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    void SetUserLevel(UserLevel level)
    {
        int levelIndex = (int)level;
        if (orgWave == null)
        {
            orgWave = new Wave[waves.Length];
            for (int i = 0; i < waves.Length; i++)
            {
                orgWave[i] = (Wave)waves[i].Clone();
            }
        }
        for (int i = 0; i < orgWave.Length; i++)
        {
            waves[i].enemyCount = (int)(orgWave[i].enemyCount * levelOfDefficult[levelIndex].enemyCountRatio);
            waves[i].timeBetweenSpawns = orgWave[i].timeBetweenSpawns * levelOfDefficult[levelIndex].spawnTimeRatio;
            waves[i].moveSpeed = orgWave[i].moveSpeed * levelOfDefficult[levelIndex].moveSpeedRatio;
            waves[i].hitsToKillPlayer = (int)(orgWave[i].hitsToKillPlayer * levelOfDefficult[levelIndex].hitsToKillPlayerRatio);
            waves[i].enemyHealth = orgWave[i].enemyHealth * levelOfDefficult[levelIndex].enemyHealthRatio;
        }
    }
    [System.Serializable]
    public class LevelOfDefficult
    {
        [Range(0, 1)]
        public float enemyCountRatio;
        [Range(0, 1)]
        public float spawnTimeRatio;
        [Range(0, 1)]
        public float moveSpeedRatio;
        [Range(1, 10)]
        public float hitsToKillPlayerRatio;
        [Range(0, 1)]
        public float enemyHealthRatio;
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;

        public object Clone()
        {
            Wave newWave = new Wave();
            newWave.infinite = this.infinite;
            newWave.enemyCount = this.enemyCount;
            newWave.timeBetweenSpawns = this.timeBetweenSpawns;

            newWave.moveSpeed = this.moveSpeed;
            newWave.hitsToKillPlayer = this.hitsToKillPlayer;
            newWave.enemyHealth = this.enemyHealth;
            newWave.skinColor = this.skinColor;
            return newWave;
    }
    }

	

}

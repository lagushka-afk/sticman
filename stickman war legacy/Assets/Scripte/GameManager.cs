using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
   
    public static GameManager Instance;
    
    public Transform LeftSpawn;
    public Transform RightSpawn;
    public GameObject LeftBase;
    public GameObject RightBase;

  
    public GameObject minerp;
    public GameObject swordp;
    public GameObject archerp;
    public GameObject tankp;

    
    
    public int MinerPrice = 25;
    public int SwordPrice = 50;
    public int ArcherPrice = 75;
    public int TankPrice = 150;

   
    public int ColvoM = 5;
    public int ColvoU = 7;

   
    public float botMinSpawnDelay = 2f;
    public float botMaxSpawnDelay = 5f;
    public int botStartGold = 100;
    public float botGoldRate = 1f;

  
    public int minerIncome = 10;
    public float minerInterval = 3f;

    public Text playerGoldText;
    public Text minerCountText;
    public Text warriorCountText;

    public Button minerButton;
    public Button swordsmanButton;
    public Button archerButton;
    public Button tankButton;

    public Text minerPriceText;
    public Text swordsmanPriceText;
    public Text archerPriceText;
    public Text tankPriceText;

    private int playerGold = 100;
    private int botGold = 100;
    private int currentMinerCount = 0;
    private int currentWarriorCount = 0;

    private List<GameObject> playerMiners = new List<GameObject>();
    private List<GameObject> playerWarriors = new List<GameObject>();
    private List<GameObject> botUnits = new List<GameObject>();

    private float botSpawnTimer;
    private float nextBotSpawnTime;

    void Start()
    {
        Instance = this;
        minerPriceText.text = MinerPrice.ToString();
        swordsmanPriceText.text = SwordPrice.ToString();
        archerPriceText.text = ArcherPrice.ToString();
        tankPriceText.text = TankPrice.ToString();

         
        minerButton.onClick.AddListener(SpawnMiner);
        swordsmanButton.onClick.AddListener(SpawnSwordsman);
        archerButton.onClick.AddListener(SpawnArcher);
        tankButton.onClick.AddListener(SpawnTank);

          
        nextBotSpawnTime = Random.Range(botMinSpawnDelay, botMaxSpawnDelay);
        botSpawnTimer = 0;
        botGold = botStartGold;
    }

    void Update()
    {
        
        playerGoldText.text = $"Золото: {playerGold}";
        minerCountText.text = $"Шахтеры: {currentMinerCount}/{ColvoM}";
        warriorCountText.text = $"Воины: {currentWarriorCount}/{ColvoU}";

           
        botGold += (int)(botGoldRate * Time.deltaTime);

        
        botSpawnTimer += Time.deltaTime;
        if (botSpawnTimer >= nextBotSpawnTime)
        {
            botSpawnTimer = 0;
            nextBotSpawnTime = Random.Range(botMinSpawnDelay, botMaxSpawnDelay);
            BotSpawn();
        }
    }

        
    public void SpawnMiner()
    {
        if (currentMinerCount >= ColvoM) return;
        if (playerGold < MinerPrice) return;

        playerGold -= MinerPrice;
        currentMinerCount++;

        GameObject miner = Instantiate(minerp, LeftSpawn.position, Quaternion.identity);
        miner.tag = "PlayerUnit";
        
playerMiners.Add(miner);

        MinerController minerScript = miner.AddComponent<MinerController>();
        minerScript.Initialize(minerIncome, minerInterval, "Player");
    }

    public void SpawnSwordsman()
    {
        if (currentWarriorCount >= ColvoU) return;
        if (playerGold < SwordPrice) return;

        playerGold -= SwordPrice;
        currentWarriorCount++;

        SpawnWarrior(swordp, 20, 1f, 1.5f, 100, "Player");
    }

    public void SpawnArcher()
    {
        if (currentWarriorCount >= ColvoU) return;
        if (playerGold < ArcherPrice) return;

        playerGold -= ArcherPrice;
        currentWarriorCount++;

        SpawnWarrior(archerp, 15, 1.5f, 3f, 75, "Player");
    }

    public void SpawnTank()
    {
        if (currentWarriorCount >= ColvoU) return;
        if (playerGold < TankPrice) return;

        playerGold -= TankPrice;
        currentWarriorCount++;

        SpawnWarrior(tankp, 30, 0.5f, 1f, 200, "Player");
    }

    void SpawnWarrior(GameObject prefab, int damage, float speed, float range, int health, string team)
    {
        Transform spawnPoint = team == "Player" ? LeftSpawn : RightSpawn;
        Transform enemyBase = team == "Player" ? RightBase.transform : LeftBase.transform;

        GameObject warrior = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        warrior.tag = team == "Player" ? "PlayerUnit" : "BotUnit";

        WarriorController warriorScript = warrior.AddComponent<WarriorController>();
        warriorScript.Initialize(damage, speed, range, health, team, enemyBase);

        if (team == "Player")
            playerWarriors.Add(warrior);
        else
            botUnits.Add(warrior);
    }

    
    void BotSpawn()
    {
        
        botUnits.RemoveAll(u => u == null);

        if (botUnits.Count >= 7) return;

        if (botGold < 50)
        {
            return; 
        }
        else if (botGold < 75)
        {
            if (CountBotMiners() < 5)
                SpawnBotMiner();
        }
        else if (botGold < 150)
        {
            if (Random.value > 0.5f)
                SpawnBotUnit(swordp);
            else
                SpawnBotUnit(archerp);
        }
        else
        {
            if ((currentMinerCount + currentWarriorCount) > 5 && Random.value > 0.7f)
                SpawnBotUnit(tankp);
            else
            {
                float rand = Random.value;
                if (rand < 0.3f) SpawnBotMiner();
                else if (rand < 0.6f) SpawnBotUnit(swordp);
                else if (rand < 0.85f) SpawnBotUnit(archerp);
                else SpawnBotUnit(tankp);
            }
        }
    }

    void SpawnBotMiner()
    {
        if (botGold < MinerPrice) return;
        botGold -= MinerPrice;

        GameObject miner = Instantiate(minerp, RightSpawn.position, Quaternion.identity);
        miner.tag = "BotUnit";
        botUnits.Add(miner);

        MinerController minerScript = miner.AddComponent<MinerController>();
        minerScript.Initialize(minerIncome, minerInterval, "Bot");
    }

    void SpawnBotUnit(GameObject prefab)
    {
        int cost = 0;
        int damage = 0;
        float speed = 0;
        float range = 0;
        int health = 0;

        if (prefab == swordp)
        {
            cost = SwordPrice; damage = 20; speed = 1f; range = 1.5f; health = 100;
        }
        else if (prefab == archerp)
        {
            cost = ArcherPrice; damage = 15; speed = 1.5f; range = 3f; health = 75;
        }
        else if (prefab == tankp)
        {
            cost = TankPrice; damage = 30; speed = 0.5f; range = 1f; health = 200;
        }
        
if (botGold < cost) return;
        botGold -= cost;

        SpawnWarrior(prefab, damage, speed, range, health, "Bot");
    }

    int CountBotMiners()
    {
        int count = 0;
        foreach (GameObject u in botUnits)
        {
            if (u != null && u.GetComponent<MinerController>() != null)
                count++;
        }
        return count;
    }

    
    public void AddGold(int amount, string team)
    {
        if (team == "Player") playerGold += amount;
        else botGold += amount;
    }

    public void RemoveUnit(GameObject unit, string type)
    {
        if (type == "Miner" && unit.tag == "PlayerUnit")
        {
            playerMiners.Remove(unit);
            currentMinerCount--;
        }
        else if (type == "Warrior" && unit.tag == "PlayerUnit")
        {
            playerWarriors.Remove(unit);
            currentWarriorCount--;
        }
        else if (unit.tag == "BotUnit")
        {
            botUnits.Remove(unit);
        }
        Destroy(unit);
    }
}


public class MinerController : MonoBehaviour
{
    private int income;
    private float interval;
    private string team;
    private float timer;

    public void Initialize(int goldPerTick, float tickInterval, string unitTeam)
    {
        income = goldPerTick;
        interval = tickInterval;
        team = unitTeam;
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            GameManager.Instance.AddGold(income, team);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RemoveUnit(gameObject, "Miner");
    }
}

public class WarriorController : MonoBehaviour
{
    private int damage;
    private float speed;
    private float range;
    private int MaxHealth;
    private int CurrentHealth;
    private string team;
    private Transform EnemyBase;
    private Transform target;
    private float lastAttackTime;
    private float attackCooldown = 1f;

    public void Initialize(int dmg, float spd, float rng, int hp, string tm, Transform enemyBaseTransform)
    {
        damage = dmg;
        speed = spd;
        range = rng;
        MaxHealth = hp;
        CurrentHealth = hp;
        team = tm;
        EnemyBase = enemyBaseTransform;
        lastAttackTime = Time.time;
    }

    void Update()
    {
        FindTarget();
        Move();
        TryAttack();
    }

    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(
            team == "Player" ? "BotUnit" : "PlayerUnit"
        );

        target = null;
        float closestDistance = range;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemy.transform;
            }
        }

        if (target == null)
            target = EnemyBase;
    }

    void Move()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > range)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target.position, speed * Time.deltaTime);
        }
    }

    void TryAttack()
    {
        if (target == null) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= range)
        {
            lastAttackTime = Time.time;
            
if (target.CompareTag("BotUnit") || target.CompareTag("PlayerUnit"))
            {
                WarriorController enemy = target.GetComponent<WarriorController>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
            else if (target.CompareTag("Base"))
            {
                BaseHealth baseHealth = target.GetComponent<BaseHealth>();
                if (baseHealth != null) baseHealth.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;
        StartCoroutine(FlashRed());

        if (CurrentHealth <= 0)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RemoveUnit(gameObject, "Warrior");
        }
    }

    IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Image img = GetComponent<Image>();

        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
        else if (img != null)
        {
            img.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            img.color = Color.white;
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RemoveUnit(gameObject, "Warrior");
    }
}


public class BaseHealth : MonoBehaviour
{
    public int MaxHealth = 1000;
    private int CurrentHealth;

    void Start()
    {
        CurrentHealth = MaxHealth;
        gameObject.tag = "Base";
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("База уничтожена!");
        }
    }
}
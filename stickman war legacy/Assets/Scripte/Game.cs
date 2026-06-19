using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public Transform GameWorld;
    public RectTransform LeftSpawn;
    public RectTransform RightSpawn;
    public RectTransform LeftBase;
    public RectTransform RightBase;
    public GameObject MinerPrefab;
    public GameObject SwordPrefab;
    public GameObject ArcherPrefab;
    public GameObject TankPrefab;
    public int MinerPrice = 25;
    public int SwordPrice = 50;
    public int ArcherPrice = 75;
    public int TankPrice = 150;
    public int MaxMiners = 5;
    public int MaxWarriors = 7;
    public float BotSpawnDelay = 3f;
    public int MinerIncome = 10;
    public float MinerInterval = 3f;
    public Text PlayerGoldText;
    public Text MinerCountText;
    public Text WarriorCountText;
    public Button MinerButton;
    public Button SwordButton;
    public Button ArcherButton;
    public Button TankButton;
    public Text MinerPriceText;
    public Text SwordPriceText;
    public Text ArcherPriceText;
    public Text TankPriceText;

    private int PlayerGold = 100;
    private int CurrentMinerCount = 0;
    private int CurrentWarriorCount = 0;
    private List<GameObject> PlayerMiners = new List<GameObject>();
    private List<GameObject> PlayerWarriors = new List<GameObject>();
    private List<GameObject> BotUnits = new List<GameObject>();
    private float BotSpawnTimer;

    void Awake() { Instance = this; }

    void Start()
    {
        MinerPriceText.text = MinerPrice.ToString();
        SwordPriceText.text = SwordPrice.ToString();
        ArcherPriceText.text = ArcherPrice.ToString();
        TankPriceText.text = TankPrice.ToString();
        MinerButton.onClick.AddListener(SpawnMiner);
        SwordButton.onClick.AddListener(SpawnSword);
        ArcherButton.onClick.AddListener(SpawnArcher);
        TankButton.onClick.AddListener(SpawnTank);
        BotSpawnTimer = BotSpawnDelay;
    }

    void Update()
    {
        PlayerGoldText.text = $"Золото: {PlayerGold}";
        MinerCountText.text = $"Шахтеры: {CurrentMinerCount}/{MaxMiners}";
        WarriorCountText.text = $"Воины: {CurrentWarriorCount}/{MaxWarriors}";
        BotSpawnTimer -= Time.deltaTime;
        if (BotSpawnTimer <= 0) { BotSpawnTimer = BotSpawnDelay; BotSpawn(); }
    }

    public void SpawnMiner()
    {
        if (CurrentMinerCount >= MaxMiners || PlayerGold < MinerPrice) return;
        PlayerGold -= MinerPrice;
        CurrentMinerCount++;
        GameObject Obj = Instantiate(MinerPrefab, GameWorld.transform);
        Obj.GetComponent<RectTransform>().anchoredPosition = LeftSpawn.anchoredPosition;
        PlayerMiners.Add(Obj);
        Obj.AddComponent<Miner>().Init(MinerIncome, MinerInterval, "Player");
    }

    public void SpawnSword()
    {
        if (CurrentWarriorCount >= MaxWarriors || PlayerGold < SwordPrice) return;
        PlayerGold -= SwordPrice;
        CurrentWarriorCount++;
        MakeWarrior(SwordPrefab, 20, 1f, 1.5f, 100, "Player");
    }

    public void SpawnArcher()
    {
        if (CurrentWarriorCount >= MaxWarriors || PlayerGold < ArcherPrice) return;
        PlayerGold -= ArcherPrice;
        CurrentWarriorCount++;
        MakeWarrior(ArcherPrefab, 15, 1.5f, 3f, 75, "Player");
    }

    public void SpawnTank()
    {
        if (CurrentWarriorCount >= MaxWarriors || PlayerGold < TankPrice) return;
        PlayerGold -= TankPrice;
        CurrentWarriorCount++;
        MakeWarrior(TankPrefab, 30, 0.5f, 1f, 200, "Player");
    }

    void MakeWarrior(GameObject Prefab, int Damage, float Speed, float Range, int Health, string Team)
    {
        RectTransform SpawnPoint = Team == "Player" ? LeftSpawn : RightSpawn;
        RectTransform EnemyBase = Team == "Player" ? RightBase : LeftBase;
        GameObject Obj = Instantiate(Prefab, GameWorld.transform);
        Obj.GetComponent<RectTransform>().anchoredPosition = SpawnPoint.anchoredPosition;
        Obj.AddComponent<Unit>().Init(Damage, Speed, Range, Health, Team, EnemyBase);
        if (Team == "Player") PlayerWarriors.Add(Obj);
        else BotUnits.Add(Obj);
    }

    void BotSpawn()
    {
        BotUnits.RemoveAll(U => U == null);
        if (BotUnits.Count >= 7) return;
        float Rand = Random.value;
        if (Rand < 0.25f) BotMakeMiner();
        else if (Rand < 0.55f) BotMakeUnit(SwordPrefab);
        else if (Rand < 0.85f) BotMakeUnit(ArcherPrefab);
        else BotMakeUnit(TankPrefab);
    }

    void BotMakeMiner()
    {
        GameObject Obj = Instantiate(MinerPrefab, GameWorld.transform);
        Obj.GetComponent<RectTransform>().anchoredPosition = RightSpawn.anchoredPosition;
        BotUnits.Add(Obj);
        Obj.AddComponent<Miner>().Init(MinerIncome, MinerInterval, "Bot");
    }

    void BotMakeUnit(GameObject Prefab)
    {
        int Damage = 0, Health = 0;
        float Speed = 0, Range = 0;
        if (Prefab == SwordPrefab) { Damage = 20; Speed = 1f; Range = 1.5f; Health = 100; }
        else if (Prefab == ArcherPrefab) { Damage = 15; Speed = 1.5f; Range = 3f; Health = 75; }
        else if (Prefab == TankPrefab) { Damage = 30; Speed = 0.5f; Range = 1f; Health = 200; }
        MakeWarrior(Prefab, Damage, Speed, Range, Health, "Bot");
    }

    public void AddGold(int Amount, string Team)
    {
        if (Team == "Player") PlayerGold += Amount;
    }

    public void Remove(GameObject Obj, string Type)
    {
        if (Type == "Miner" && PlayerMiners.Contains(Obj)) { PlayerMiners.Remove(Obj); CurrentMinerCount--; }
        else if (Type == "Unit" && PlayerWarriors.Contains(Obj)) { PlayerWarriors.Remove(Obj); CurrentWarriorCount--; }
        else if (BotUnits.Contains(Obj)) { BotUnits.Remove(Obj); }
        Destroy(Obj);
    }
}
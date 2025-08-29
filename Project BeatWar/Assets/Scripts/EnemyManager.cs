using System.Collections.Generic;
using SeolMJ;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public readonly Dictionary<Enemy, Stack<Enemy>> Pools = new();

    [Header("Settings")]
    [SerializeField]
    int interval;

    [Header("Resources")]
    [SerializeField]
    Enemy[] enemies;

    public ParticleEmitCaller OnHitCaller { get; private set; }

    WeightedRandom enemyWeights;

    int currentBeat = 0;

    void Awake()
    {
        Instance = this;
        OnHitCaller = GetComponent<ParticleEmitCaller>();
        SetEnemyWeights();
    }

    void SetEnemyWeights()
    {
        enemyWeights = new();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyWeights.Register(i, enemies[i].Weight);
        }
    }

    public void Begin()
    {
        currentBeat = BeatManager.Instance.CurrentBeat;
    }

    void Update()
    {
        while (StageManager.Instance.Started
            && BeatManager.Instance.CurrentBeat >= currentBeat + interval)
        {
            currentBeat = BeatManager.Instance.CurrentBeat;
            SpawnEnemies();
            //SpawnRandomEnemies();
        }
    }

    void SpawnEnemies()
    {
        int cursor = currentBeat + 1;
        if (cursor >= StageManager.Track.Chart.Length)
        {
            return;
        }
        Pattern pattern = StageManager.Track.Chart[cursor];
        if (!WeaponManager.Instance.PatternTable.TryGetValue(pattern.Count(), out Weapon weapon))
        {
            return;
        }
        Enemy enemy = Borrow(FindEnemyOf(weapon.Damage));
        StageManager.Instance.Locate(enemy, StageManager.Track.Lanes.Random());
    }

    void SpawnRandomEnemies()
    {
        for (int i = 0; i < 8; i++)
        {
            if (StageManager.Track.Lanes.HasLane(i))
            {
                if (Random.value < StageManager.Track.SpawnRate(BeatManager.Instance.Beat))
                {
                    Enemy enemy = Borrow(enemies[enemyWeights.Get()]);
                    StageManager.Instance.Locate(enemy, i);
                }
            }
        }
    }

    //기존에 health에 따라 다른 적 불러옴. 지금은 강제로 첫번째 적 (health=1)만 불러오게 해놓음
    Enemy FindEnemyOf(int health)
    {
        health = 1;
        foreach (Enemy enemy in enemies)
        {
            if (enemy.Health == health)
            {
                return enemy;
            }
        }
        SLogger.LogError($"Enemy with health={health} not found");
        return enemies[0];
    }

    public Enemy Borrow(Enemy enemy)
    {
        if (!Pools.TryGetValue(enemy, out Stack<Enemy> pool))
        {
            pool = new();
            Pools[enemy] = pool;
        }
        if (!pool.TryPop(out Enemy instance))
        {
            instance = Instantiate(enemy, transform);
        }
        instance.Enable(enemy);
        return instance;
    }

    public void Return(Enemy instance)
    {
        if (!Pools.TryGetValue(instance.Original, out Stack<Enemy> pool))
        {
            pool = new();
            Pools[instance.Original] = pool;
        }
        instance.Disable();
        pool.Push(instance);
    }
}

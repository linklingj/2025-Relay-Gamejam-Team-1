using System.Collections.Generic;
using SeolMJ;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public readonly Dictionary<Enemy, Stack<Enemy>> Pools = new();

    [Header("Settings")]
    [SerializeField]
    // 몹을 소환하는 간격
    int interval;
    
    [SerializeField]
    // 한 번에 소환하는 몹의 수
    int enemiesPerSpawn = 1;
    
    [SerializeField]
    bool randomLane = false;

    [Header("Resources")]
    [SerializeField]
    Enemy[] enemies;

    public ParticleEmitCaller OnHitCaller { get; private set; }

    WeightedRandom enemyWeights;
    
    [SerializeField]
    private int _currentBeat;
    private int cursor = 0;

    void Awake()
    {
        Instance = this;
        OnHitCaller = GetComponent<ParticleEmitCaller>();
        SetEnemyWeights();
        cursor = 0;
    }

    void SetEnemyWeights()
    {
        enemyWeights = new WeightedRandom();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyWeights.Register(i, enemies[i].Weight);
        }
    }

    public void Setup(Track track)
    {
        // setup interval and enemiesPerSpawn
        interval = track.Interval;
        enemiesPerSpawn = track.enemiesPerSpawn;
        randomLane = track.randomizeLanes;
        _currentBeat = track.patternStartBeat - interval;
    }

    void Update()
    {
        while (StageManager.Instance.Started
            && BeatManager.Instance.CurrentBeat >= _currentBeat + interval)
        {
//            SLogger.Log($"Spawning enemies at beat {BeatManager.Instance.CurrentBeat}");
            
            _currentBeat = BeatManager.Instance.CurrentBeat;
            SpawnEnemies();
            //SpawnRandomEnemies();
        }
    }

    void SpawnEnemies()
    {
        if (cursor >= StageManager.Track.Chart.Length)
        {
            return;
        }
        
        int[] lanes = randomLane ?
            StageManager.Track.Lanes.Randoms(enemiesPerSpawn) :
            StageManager.Track.Lanes.GetRange(enemiesPerSpawn);
        
        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            if (cursor >= StageManager.Track.Chart.Length)
            {
                SLogger.Log("No more patterns to spawn");
                return;
            }
            
            Pattern pattern = StageManager.Track.Chart[cursor++];
            if (!WeaponManager.Instance.PatternTable.TryGetValue(pattern.Count(), out Weapon weapon))
            {
                if(pattern.Count() > 0)SLogger.LogError($"No weapon for pattern of length {pattern.Count()}");
                continue;
            }
            Enemy enemy = Borrow(FindEnemyOf(weapon.Damage));
            
            enemy.SetPattern(pattern);
            
            StageManager.Instance.Locate(enemy, lanes[i]);
        }
        
    }

    void SpawnRandomEnemies()
    {
        for (int i = 0; i < 8; i++)
        {
            if (StageManager.Track.Lanes.HasLane(i))
            {
                if (Random.value < StageManager.Track.SpawnRate(BeatManager.Instance.Beat01))
                {
                    // Enemy enemy = Borrow(enemies[enemyWeights.Get()]);   //강제로 0번째 적만 나오게 함
                    Enemy enemy = Borrow(enemies[0]);
                    StageManager.Instance.Locate(enemy, i);
                }
            }
        }
    }

    //기존에 health에 따라 다른 적 불러옴. 
    Enemy FindEnemyOf(int health)
    {
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

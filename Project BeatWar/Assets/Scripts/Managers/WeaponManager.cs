using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public readonly Dictionary<int, Weapon> PatternTable = new();
    public readonly Dictionary<Weapon, Stack<Weapon>> Pools = new();

    [field: Header("Resources")]
    [field: SerializeField]
    public Weapon[] Weapons { get; private set; }

    void Awake()
    {
        Instance = this;
        IndexWeapons();
    }

    void IndexWeapons()
    {
        foreach (Weapon weapon in Weapons)
        {
            PatternTable.Add(weapon.Damage, weapon);
        }
    }

    int beat;

    public void Begin()
    {
        beat = BeatManager.Instance.CurrentBeat;
    }

    void Update()
    {
        if (StageManager.Instance.Paused) return;

        if (StageManager.Instance.Started
            && BeatManager.Instance.CurrentBeat > beat)
        {
            beat = BeatManager.Instance.CurrentBeat;
            RunWeapons();
        }
    }

    void RunWeapons()
    {
        if (Weapon.Instances.Count == 0)
        {
            return;
        }
        var current = Weapon.Instances
            .Where(w => w.Beat < BeatManager.Instance.CurrentBeat)
            .ToArray();
        foreach (var weapon in current)
        {
            weapon.Attack();
        }
        foreach (var weapon in current)
        {
            if (weapon == null) continue;
            
            weapon.Disable();
            Return(weapon);
        }
    }

    public Weapon Borrow(Pattern pattern, Lane lane)
    {
        if (!PatternTable.TryGetValue(pattern.Count(), out Weapon weapon))
        {
            return null;
        }
        if (!Pools.TryGetValue(weapon, out Stack<Weapon> pool))
        {
            pool = new();
            Pools[weapon] = pool;
        }
        if (!pool.TryPop(out Weapon instance))
        {
            instance = Instantiate(weapon, transform);
        }
        instance.Enable(pattern, lane);
        return instance;
    }

    public void Return(Weapon instance)
    {
        if (!PatternTable.TryGetValue(instance.Damage, out Weapon weapon))
        {
            return;
        }
        if (!Pools.TryGetValue(weapon, out Stack<Weapon> pool))
        {
            pool = new();
            Pools[weapon] = pool;
        }
        instance.Disable();
        pool.Push(instance);
    }
}

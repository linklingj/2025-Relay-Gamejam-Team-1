using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Scrolled
{
    public static List<Entity> Entities = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnInit()
    {
        Entities.Clear();
    }

    public int Lane
    {
        get => Mathf.RoundToInt(PositionX / StageManager.Instance.LaneSpace + 3.5f);
    }

    [field: SerializeField]
    public int Health { get; private set; }

    public void Kill()
    {
        Disable();
    }

    [field: SerializeField]
    public float Weight { get; private set; }

    public virtual void Enable()
    {
        Entities.Add(this);
        gameObject.SetActive(true);
    }

    public virtual void Disable()
    {
        Entities.Remove(this);
        gameObject.SetActive(false);
    }

    public static Entity Scan(int lane, int health)
    {
        float minY = float.PositiveInfinity;
        Entity target = null;
        foreach (Entity entity in Entities)
        {
            if (entity == null) continue;
            
            if (entity.Lane == lane && entity.PositionY < minY && entity.Health >= health)
            {
                minY = entity.PositionY;
                target = entity;
            }
        }
        return target;
    }
}

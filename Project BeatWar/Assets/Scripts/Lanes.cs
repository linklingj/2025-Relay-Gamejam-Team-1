using UnityEngine;

[System.Flags]
public enum Lanes : byte
{
    None = 0,
    A = 1 << 0,
    B = 1 << 1,
    C = 1 << 2,
    D = 1 << 3,
    E = 1 << 4,
    F = 1 << 5,
    G = 1 << 6,
    H = 1 << 7,
    All = A | B | C | D | E | F | G | H,
}

public static class LaneUtils
{
    public static Lanes ToLane(this int index)
    {
        return (Lanes)(1 << index);
    }

    public static bool HasLane(this Lanes lanes, int index)
    {
        return (((byte)lanes >> index) & 1) == 1;
    }

    public static int Count(this Lanes lanes)
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            if (lanes.HasLane(i))
            {
                count++;
            }
        }
        return count;
    }

    public static int Random(this Lanes lanes)
    {
        int count = lanes.Count();
        if (count == 0) return 0;
        int random = UnityEngine.Random.Range(0, count);
        for (int i = 0, j = 0; i < 8; i++)
        {
            if (lanes.HasLane(i))
            {
                if (j++ == random)
                {
                    return i;
                }
            }
        }
        return 0;
    }
}
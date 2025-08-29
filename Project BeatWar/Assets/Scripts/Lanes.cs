using System;
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

    public static int[] GetRange(this Lanes lanes, int size)
    {
        int count = lanes.Count();
        if (size > count) size = count;
        
        if (count == 0) return Array.Empty<int>();
        
        int[] result = new int[size];
        for (int i = 0, j = 0; i < 8 && j < size; i++)
        {
            if (lanes.HasLane(i))
            {
                result[j++] = i;
            }
        }
        return result;
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

    // 중복되지 않는 랜덤한 lane 인덱스 배열 반환
    public static int[] Randoms(this Lanes lanes, int cnt)
    {
        int count = lanes.Count();
        if (count == 0 || cnt <= 0) return Array.Empty<int>();
        if (cnt > count) cnt = count;
        int[] result = new int[cnt];
        bool[] chosen = new bool[8];
        for (int i = 0; i < cnt; i++)
        {
            int random;
            do
            {
                random = UnityEngine.Random.Range(0, count);
            } while (chosen[random]);
            
            chosen[random] = true;
            for (int j = 0, k = 0; j < 8; j++)
            {
                if (lanes.HasLane(j))
                {
                    if (k++ == random)
                    {
                        result[i] = j;
                        break;
                    }
                }
            }
        }
        return result;
    }
}
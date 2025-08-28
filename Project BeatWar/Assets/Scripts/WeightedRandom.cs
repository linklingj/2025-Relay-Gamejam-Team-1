using System.Collections.Generic;
using UnityEngine;

public class WeightedRandom
{
    public readonly List<(int Index, float Weight)> weights = new();

    bool dirty = true;
    float totalWeight;

    public void Register(int index, float weight)
    {
        weights.Add((index, weight));
        dirty = true;
    }

    public int Get()
    {
        if (weights.Count == 0)
        {
            return -1;
        }
        if (dirty)
        {
            dirty = false;
            totalWeight = 0f;
            foreach (var (_, weight) in weights)
            {
                totalWeight += weight;
            }
        }
        float random = Random.Range(0f, totalWeight);
        for (int i = 0; i < weights.Count; i++)
        {
            var (index, weight) = weights[i];
            if (random <= weight)
            {
                return index;
            }
            else
            {
                random -= weight;
            }
        }
        return weights[^1].Index;
    }
}

using UnityEngine;

public class PatternIndicator : MonoBehaviour
{
    [SerializeField]
    PatternDot[] dots;

    Lane lane;
    Enemy enemy;

    void Awake()
    {
        lane = GetComponentInParent<Lane>();
        enemy = GetComponentInParent<Enemy>();
    }

    void Update()
    {
        Pattern pattern = default;
        if (lane != null)
        {
            pattern = lane.Pattern;
        }
        else if (enemy != null)
        {
            pattern = enemy.Weakness;
        }
        for (int i = 0; i < 4; i++)
        {
            dots[i].Set(pattern.HasPattern(i));
        }
    }
}

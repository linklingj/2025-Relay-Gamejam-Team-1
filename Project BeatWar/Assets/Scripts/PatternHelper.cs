using SeolMJ;
using UnityEngine;

public class PatternHelper : MonoBehaviour
{
    private Pattern currentPattern = Pattern.None;
    
    [SerializeField] private PatternDot[] dots;
    
    private void Update()
    {
        Pattern subBeatPattern = BeatManager.Instance.InputSubBeat.ToPattern();
        for (int i = 0; i < 4; i++)
        {
            dots[i].Set(subBeatPattern.HasPattern(i));
        }
    }
}

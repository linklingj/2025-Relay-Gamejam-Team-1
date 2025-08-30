using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    void Update()
    {
        int score = ScoreSystem.CurrentScore;
        
        GetComponent<TMP_Text>().text = "Score " + score.ToString("D9");
    }
}

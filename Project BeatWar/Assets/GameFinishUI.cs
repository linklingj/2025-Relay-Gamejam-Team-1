using TMPro;
using UnityEngine;

public class GameFinishUI : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text resultText;
    
    public string winText = "Clear!";
    public string loseText = "PanCu. Kim (2003 - 2025)";
    
    private void Awake()
    {
        StageManager.Instance.OnStageEnd += OnFinish;
    }

    private void OnFinish(bool arg0)
    {
        gameObject.SetActive(true);
        
        resultText.text = arg0 ? winText : loseText;
        
        score.text = $"Score: {ScoreSystem.CurrentScore}";
    }
}


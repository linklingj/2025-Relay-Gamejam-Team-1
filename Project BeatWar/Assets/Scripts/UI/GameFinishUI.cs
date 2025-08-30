using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GameFinishUI : MonoBehaviour
{
    private CanvasGroup group;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text resultText;

    public string winText = "Win!";
    public string loseText = "PanCu. Kim (2003 - 2025)";

    bool shown;

    private void Reset()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        Hide();
    }

    // 나도 이따구로 짜고싶지 않았는데 이벤트 핸들러가 시발 문제가 많아 유니티6 병신새끼들아
    // ??누구세요
    public void Update()
    {
        switch (StageManager.Instance.Ended)
        {
            case true when !shown:
                OnFinish(StageManager.Instance.IsWin);
                break;
            case false when shown:
                Hide();
                break;
        }
    }
    
    void Hide()
    {
        if (!group) return;
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
        shown = false;
    }

    void Show(bool isWin)
    {
        if (!group) return;
        resultText.text = isWin ? winText : loseText;
        score.text = $"Score: {ScoreSystem.CurrentScore}";
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = true;
        shown = true;
    }

    void OnFinish(bool isWin)
    {
        // 중복 호출/초기 즉시 콜백 방지 가드가 필요하면:
        // if (shown) return;
        Show(isWin);
    }
}
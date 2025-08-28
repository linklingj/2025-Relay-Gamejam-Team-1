using SeolMJ;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FlashUI : MonoBehaviour
{
    [SerializeField]
    float duration;

    [SerializeField]
    AnimationCurve ease;

    CanvasGroup canvasGroup;

    readonly Timer timer = new();

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        timer.Set();
    }

    void Update()
    {
        canvasGroup.alpha = ease.Evaluate(timer.Progress);
    }

    public void Flash()
    {
        timer.Set(duration);
    }
}

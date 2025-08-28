using UnityEngine;
using SeolMJ;

public class FadeOnLoad : MonoBehaviour
{
    const float FADE_TIME = 0.1f;

    CanvasGroup canvasGroup;

    bool isLoadEnded = false;
    readonly Timer.Unscaled fadeTimer = new();

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SceneLoader.OnLoadEnd += OnLoadEnd;
    }

    void OnLoadEnd()
    {
        SceneLoader.OnLoadEnd -= OnLoadEnd;

        isLoadEnded = true;
        fadeTimer.Set(FADE_TIME);
    }

    void Update()
    {
        if (!isLoadEnded)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 1f, fadeTimer.Progress);
        }
        else
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeTimer.Progress);
            if (fadeTimer.IsFinished)
            {
                Destroy(gameObject);
            }
        }
    }
}

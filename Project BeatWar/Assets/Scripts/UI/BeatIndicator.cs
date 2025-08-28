using UnityEngine;
using UnityEngine.UI;

public class BeatIndicator : MonoBehaviour
{
    public Image Fill;
    public FlashUI BigFlash;
    public FlashUI SmallFlash;

    int beat;
    int subBeat;

    void Update()
    {
        Fill.fillAmount = Mathf.Repeat(1f - BeatManager.Instance.Beat, 1f);

        if (beat != BeatManager.Instance.CurrentBeat)
        {
            beat = BeatManager.Instance.CurrentBeat;
            BigFlash.Flash();
        }

        if (subBeat != BeatManager.Instance.CurrentSubBeat)
        {
            subBeat = BeatManager.Instance.CurrentSubBeat;
            SmallFlash.Flash();
        }
    }
}

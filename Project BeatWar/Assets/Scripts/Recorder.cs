using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Recorder : MonoBehaviour
{
    public readonly List<Pattern> chart = new();

    Pattern pattern;
    int beat;

    void Start()
    {
        beat = BeatManager.Instance.CurrentBeat;
    }

    void Update()
    {
        if (BeatManager.Instance.InputBeat >= 0
            && beat != BeatManager.Instance.InputBeat)
        {
            beat = BeatManager.Instance.InputBeat;
            chart.Add(pattern);
            pattern = Pattern.None;
        }

        for (int i = 0; i < 8; i++)
        {
            var key = Player.Keys[i];
            var control = Keyboard.current[key];

            if (control.wasPressedThisFrame)
            {
                if (i == 0)
                {
                    StageManager.Track.Chart = chart.ToArray();
                    Application.Quit();
                    break;
                }
                int subBeat = BeatManager.Instance.InputSubBeat;
                if (!pattern.HasPattern(subBeat))
                {
                    pattern |= subBeat.ToPattern();
                }
                break;
            }
        }
    }
}

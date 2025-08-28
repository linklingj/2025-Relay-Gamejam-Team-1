using UnityEngine;

public static class BeatUtils
{
    public const int SUB_BEAT_LENGTH = 4;

    public static void RoundBeat(float bpm, float time, out int mainBeat, out int subBeat)
    {
        float bps = bpm / 60f;
        float beat = time * bps;
        mainBeat = (int)(beat + (0.5f / SUB_BEAT_LENGTH));
        subBeat = Mathf.RoundToInt(beat * SUB_BEAT_LENGTH) % SUB_BEAT_LENGTH;
    }
}

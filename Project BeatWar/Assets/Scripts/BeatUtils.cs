using System;

public static class BeatUtils
{
    public const int SUB_BEAT_LENGTH = 4;

    // bpm / elapsedSec 를 하나의 그리드 함수로 양자화
    // leadSub: 서브비트 단위의 리드(0 = floor, 0.25 = 1/4 sub 미리 당김)
    public static void Quantize(double bpm, double elapsedSec,
        out int mainBeat, out int subBeat,
        double leadSub = 0.0, int subPerBeat = SUB_BEAT_LENGTH)
    {
        double subPos = elapsedSec * (bpm / 60.0) * subPerBeat + leadSub;

        // 경계 지터 억제용 작은 epsilon
        int subIndex = (int)Math.Floor(subPos + 1e-9);
        if (subIndex < 0) subIndex = 0;

        mainBeat = subIndex / subPerBeat;
        subBeat  = subIndex % subPerBeat;
    }

    public static double BeatExact(double bpm, double elapsedSec)
        => elapsedSec * (bpm / 60.0);
}
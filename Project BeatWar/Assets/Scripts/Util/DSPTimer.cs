using UnityEngine;

/// <summary>
/// 오디오 DSP 시계를 사용하는 타이머.
/// - AudioSource를 넘기면 clip의 실제 재생 위치(timeSamples)를 기준으로 흐름.
/// - AudioSource를 넘기지 않으면 AudioSettings.dspTime을 기준으로 흐름(언스케일드, 초 정밀).
/// </summary>
public class DSPTimer : ITimer
{
    readonly AudioSource source;
    readonly bool lockToClip; // true면 clip 위치(timeSamples/freq)를 '시계'로 사용
    double startSec;
    double durationSec = double.PositiveInfinity;

    // 루프/시크 감지를 위한 상태
    int lastSamples = -1;
    double loopOffsetSec; // 루프할 때 누적되는 길이(초)

    public DSPTimer(AudioSource source = null, bool lockToClipPosition = true)
    {
        this.source = source;
        this.lockToClip = (source != null) && lockToClipPosition;
    }

    // --- ITimer ----
    public float StartTime
    {
        get => (float)startSec;
        set => SetStartTime(value);
    }

    public float Duration
    {
        get => double.IsPositiveInfinity(durationSec) ? float.PositiveInfinity : (float)durationSec;
        set => durationSec = float.IsPositiveInfinity(value) ? double.PositiveInfinity : value;
    }

    public float Elapsed => (float)(NowSec() - startSec);
    public float Remaining => (float)(durationSec - (NowSec() - startSec));
    public float Progress => durationSec == 0.0 ? 1f : Mathf.Clamp01((float)((NowSec() - startSec) / durationSec));
    public bool IsFinished => !double.IsPositiveInfinity(durationSec) && NowSec() >= startSec + durationSec;
    public bool IsSet => !double.IsPositiveInfinity(durationSec);

    public void Set()
    {
        startSec = NowSec();
        durationSec = 0.0;
    }

    public void Set(float duration)
    {
        startSec = NowSec();
        durationSec = duration;
    }

    public void SetStartTime(float startTime)
    {
        durationSec += startSec - startTime; // EndTime 고정
        startSec = startTime;
    }

    public void Add(float duration)
    {
        if (!IsSet) Set();
        durationSec += duration;
    }

    public void Unset()
    {
        durationSec = double.PositiveInfinity;
    }

    // --- 내부: 현재 시각(초) 계산 ---
    double NowSec()
    {
        if (lockToClip && source && source.clip)
        {
            // timeSamples는 루프 시 0으로 래핑 → 이를 감지해서 clip.length를 누적
            int samples = source.timeSamples;
            if (lastSamples >= 0 && samples < lastSamples)
            {
                // 루프 또는 역시킹. 루프라고 가정하고 length 누적.
                // (명시적 시킹을 자주 한다면 ResetWrapTracking을 외부에서 호출해 초기화할 수 있음)
                loopOffsetSec += source.clip.length;
            }

            lastSamples = samples;

            double clipSec = loopOffsetSec + (double)samples / source.clip.frequency;
            return clipSec;
        }

        // Audio DSP 전역 시계(언스케일드, 더블 정밀)
        return AudioSettings.dspTime;
    }

    /// <summary>
    /// 루프/시킹 누적 보정값을 리셋. (재생 위치 기준을 현재 상태로 재정렬하고 싶을 때 호출)
    /// </summary>
    public void ResetWrapTracking()
    {
        loopOffsetSec = 0.0;
        lastSamples = -1;
    }
}
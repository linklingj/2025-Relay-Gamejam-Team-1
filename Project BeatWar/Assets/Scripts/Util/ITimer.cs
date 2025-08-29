using UnityEngine;
public interface ITimer
{
    float StartTime { get; set; }
    float Duration { get; set; }

    float Elapsed { get; }
    float Remaining { get; }
    float Progress { get; }
    bool IsFinished { get; }
    bool IsSet { get; }

    void Set();
    void Set(float duration);
    void SetStartTime(float startTime);
    void Add(float duration);
    void Unset();
}
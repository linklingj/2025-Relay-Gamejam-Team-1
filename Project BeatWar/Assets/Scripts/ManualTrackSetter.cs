using UnityEngine;

public class ManualTrackSetter : MonoBehaviour
{
    public Track Track;

    void Awake()
    {
        StageManager.Track = Track;
    }
}

using SeolMJ;
using UnityEngine;

public class ManualTrackSetter : MonoBehaviour
{
    public CurSelected CurSelected;

    void Awake()
    {
        if (CurSelected == null)
        {
            SLogger.LogError("ManualTrackSetter", "CurSelected is not assigned.");
        }
        
        if (CurSelected.HasTrack())
            StageManager.Track = CurSelected.GetTrack();
        else
        {
            throw new System.Exception("No track selected in CurSelected at start.");
        }
    }
}

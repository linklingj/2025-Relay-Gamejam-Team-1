using SeolMJ;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CurSelected", menuName = "ScriptableObjects/CurSelected", order = 1)]
public class CurSelected : ScriptableObject
{
    [SerializeField] private Track track;
    
    public event UnityAction<Track> OnTrackSelected;
    public event UnityAction OnTrackDeselected;

    public void SetTrack(Track newTrack)
    {
        track = newTrack;
        OnTrackSelected?.Invoke(track);
        
        SLogger.Log("Select track", "Track selected: " + track.Name);
    }
    
    public Track GetTrack()
    {
        return track;
    }
    
    public bool HasTrack()
    {
        return track != null;
    }
    
    public bool IsTrack(Track checkTrack)
    {
        return track == checkTrack;
    }
    
    public void ClearTrack()
    {
        track = null;
        OnTrackDeselected?.Invoke();
        
        SLogger.Log("Deselect", "Track deselected");
    }
}
using SeolMJ;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the visual representation and interaction of a stage track in the stage selection scene.
/// </summary>
public class StageTrackView : MonoBehaviour
{
    private const int MAIN_PRIORITY = 10;
    private const int SUB_PRIORITY = 1;
    
    public Track track;
    public CinemachineCamera cam;
    public CurSelected curSelected;
    
    private void Awake()
    {
        if(track == null) Destroy(gameObject);
        if (curSelected == null)
        {
            SLogger.LogError("CurSelected Missing", 
                "CurSelected reference is missing in StageTrackView.");
            Destroy(gameObject);
        }
        
        curSelected.OnTrackSelected += ReloadCam;
        curSelected.OnTrackDeselected += ResetCam;
    }

    private void OnMouseDown()
    {
        if (curSelected.IsTrack(track))
        {
            curSelected.ClearTrack();
        }
        else
        {
            curSelected.SetTrack(track);
        }
    }
    
    [ContextMenu("Set Cam" )]
    public void SetCam()
    {
        // find in children
        cam = GetComponentInChildren<CinemachineCamera>();
        if (cam == null) Debug.LogError("No CinemachineCamera found in children");
        else Debug.Log("CinemachineCamera found in children: " + cam.name);
    }

    public void ReloadCam(Track t = null)
    {
        if (cam == null) SetCam();
        cam.Priority = curSelected.IsTrack(track) ? MAIN_PRIORITY : SUB_PRIORITY;
    }
    
    public void ResetCam()
    {
        if (cam == null) SetCam();
        cam.Priority = SUB_PRIORITY;
    }
    
    private void OnDestroy()
    {
        if (curSelected != null)
        {
            curSelected.OnTrackSelected -= ReloadCam;
            curSelected.OnTrackDeselected -= ResetCam;
        }
    }
}

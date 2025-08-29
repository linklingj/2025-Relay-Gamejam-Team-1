using System.Collections;
using System.Collections.Generic;
using SeolMJ;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    /// <summary>
    /// Current Track (set in MainMenu or SongSelectionMenu)
    /// </summary>
    public static Track Track;

    [field: Header("Entity Spawn Settings")]
    [field: SerializeField]
    public float LaneSpace { get; private set; }

    [field: Header("Scroll Settings")]
    /// <summary>
    /// m/b (meters per beat)
    /// </summary>
    [field: SerializeField]
    public float ScrollSpeed { get; private set; }

    void Awake()
    {
        Instance = this;
        Calibration.Offset = PlayerPrefs.GetFloat("Offset", 0f);
        SLogger.Log($"Calibration Loaded: Offset={Calibration.Offset}");
    }

    public void Locate(Entity entity, int lane)
    {
        entity.PositionX = LaneToPosition(lane);
    }

    public float LaneToPosition(int lane)
    {
        return (lane - 3.5f) * Instance.LaneSpace;
    }

    public bool Started { get; private set; } = false;
    public bool Ended { get; private set; } = false;
    
    private bool _canEscape = false;

    public void Begin()
    {
        Started = true;
        Ended = false;
        _canEscape = false;
        WeaponManager.Instance.Begin();
        ScoreSystem.Reset(track: Track);
        EnemyManager.Instance.Setup(track: Track);
        
        Time.timeScale = 1f;
    }

    public void End()
    {
        Ended = true;
        Time.timeScale = 0f;
        
        // 비트 흐름과 오디오를 함께 정지
        BeatManager.Instance.StopBeatFlow();
        
        StartCoroutine(AllowToEscape());
    }

    private IEnumerator AllowToEscape()
    {
        yield return new WaitForSecondsRealtime(1f);
        
        _canEscape = true;
    }

    void Update()
    {
        if (Ended)
        {
            if (PlayerInputs.TapDown)
            {
                PlayerInputs.Disable();
                SceneLoader.Load(ScenePresets.MainMenu);
            }
        }
        
        if (BeatManager.Instance.CurrentBeat >= Track.GetFinishBeat() && !Ended)
        {
            End();
        }
    }
}

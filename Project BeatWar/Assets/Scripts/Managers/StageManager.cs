using System.Collections;
using System.Collections.Generic;
using SeolMJ;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    public bool Paused { get; private set; } = false;
    public bool Ended { get; private set; } = false;
    public bool IsWin { get; private set; }

    private bool _canEscape = false;

    public event UnityAction<bool> OnStageEnd = delegate { };

    public UnityEvent OnStageBegin;
    public UnityEvent OnStagePause;

    [ButtonGroup("Stage Control")]
    [Button("Begin")]
    public void Begin()
    {
        if (!Paused) {
            Started = true;
            Ended = false;

            WeaponManager.Instance.Begin();
            _canEscape = false;
            ScoreSystem.Reset(track: Track);
            EnemyManager.Instance.Setup(track: Track);
            OnStageEnd = delegate { };
        } else {
            BeatManager.Instance.StartBeatFlow();
        }

        Paused = false;
        Time.timeScale = 1f;

        OnStageBegin?.Invoke();
    }

    [ButtonGroup("Stage Control")]
    [Button("Pause")]    
    public void Pause() {
        if (Ended) return;
        Paused = true;

        Time.timeScale = 0f;
        BeatManager.Instance.PauseBeatFlow();

        OnStagePause?.Invoke();
    }

    public void End(bool clear)
    {
        Ended = true;
        Paused = true;

        Time.timeScale = 0f;
        
        // 비트 흐름과 오디오를 함께 정지
        BeatManager.Instance.StopBeatFlow(!clear);
        
        StartCoroutine(AllowToEscape());
        OnStageEnd?.Invoke(clear);
        
        IsWin = clear;
    }

    private IEnumerator AllowToEscape()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        
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
        
        if (!Started) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (Paused) Begin();
            else Pause();
        }

        if (BeatManager.Instance.CurrentBeat >= Track.GetFinishBeat() && !Ended)
        {
            End(clear: true);
        }
    }
}

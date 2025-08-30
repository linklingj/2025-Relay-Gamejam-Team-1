using SeolMJ;
using UnityEngine;
using UnityEngine.Events;

public class Calibration : MonoBehaviour
{
    /// <summary>
    /// Input Offset by seconds.
    /// (+) if late
    /// (-) if early
    /// </summary>
    public static float Offset;

    [Header("References")]
    public AudioSource Source;
    public float BPM;

    [Header("Settings")]
    public int TestCount;

    [Header("Events")]
    public UnityEvent OnReady;
    public UnityEvent OnStart;
    public UnityEvent OnBeat;
    public UnityEvent OnTap;
    public UnityEvent OnFinish;

    LogScope logScope;
    
    [ContextMenu("Reset Offset")]
    public void ResetOffset()
    {
        Offset = 0f;
        PlayerPrefs.SetFloat("Offset", Offset);
        SLogger.Log("Offset Reset");
    }

    void Start()
    {
        logScope = SLogger.Scope("Calibration Started", "Calibration Finished");
    }

    void OnDestroy()
    {
        logScope?.Dispose();
    }

    FSM<Calibration> stateMachine;

    void Update()
    {
        stateMachine ??= new FSM<Calibration>(this).Set<WaitForStart>();
        stateMachine.Update();
    }

    class WaitForStart : State<Calibration>
    {
        public override void OnBegin(Calibration owner)
        {
            SLogger.Log("Waiting for start...");
            PlayerInputs.Enable();
        }

        public override void OnUpdate(Calibration owner)
        {
            if (PlayerInputs.TapDown)
            {
                Set<Collect>();
            }
        }
    }

    class Collect : State<Calibration>
    {
        int tapCount = 0;

        float[] timeStamp;

        readonly Timer timer = new();
        readonly Timer startTimer = new();
        readonly Timer beatTimer = new();

        public override void OnBegin(Calibration owner)
        {
            SLogger.Log("Timing Collection started");
            owner.Source.Play();
            owner.OnReady.Invoke();
            timer.Set();
            startTimer.Set(owner.Source.clip.length);
        }

        public override void OnUpdate(Calibration owner)
        {
            if (PlayerInputs.TapDown)
            {
                timeStamp ??= new float[owner.TestCount];
                timeStamp[tapCount++] = timer.Elapsed - owner.Source.clip.length;
                owner.OnTap.Invoke();
                if (tapCount >= timeStamp.Length)
                {
                    Set<Finished>();
                }
            }
            if (startTimer.IsFinished || beatTimer.IsFinished)
            {
                beatTimer.Add(60f / owner.BPM);
                owner.OnBeat.Invoke();
            }
            if (startTimer.IsFinished)
            {
                SLogger.Log("Start typing");
                startTimer.Unset();
                owner.OnStart.Invoke();
            }
        }

        public override void OnEnd(Calibration owner)
        {
            float offset = 0f;
            for (int i = 0; i < timeStamp.Length; i++)
            {
                offset += timeStamp[i] - i * (60f / owner.BPM);
            }
            Offset = offset / timeStamp.Length;
            PlayerPrefs.SetFloat("Offset", Offset);
            SLogger.Log($"Offset: {Offset:0.###}s");
        }
    }

    class Finished : State<Calibration>
    {
        public override void OnBegin(Calibration owner)
        {
            owner.OnFinish.Invoke();
        }
    }
}

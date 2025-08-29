using UnityEngine;
using SeolMJ;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [field: Header("References")]
    [field: SerializeField]
    public AudioSource Source {  get; private set; }

    public readonly Timer Timer = new();
    public float Beat => Instance.Timer.Elapsed * StageManager.Track.BPS;
    public int CurrentBeat => Mathf.FloorToInt(Beat);
    public int CurrentSubBeat => Mathf.FloorToInt(Beat * BeatUtils.SUB_BEAT_LENGTH) % BeatUtils.SUB_BEAT_LENGTH;
    public int InputBeat { get; private set; }
    public int InputSubBeat { get; private set; }

    FSM<BeatManager> stateMachine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Timer.Set();
        PlayerInputs.Enable();
    }

    void Update()
    {
        stateMachine ??= new FSM<BeatManager>(this).Set<WaitForDelay>();
        stateMachine.Update();
    }

    class WaitForDelay : State<BeatManager>
    {
        public override void OnBegin(BeatManager owner)
        {
            float delay = StageManager.Track.Delay * StageManager.Track.SPB;
            owner.Timer.Set(delay);
            owner.Timer.SetStartTime(Time.time + delay);
            owner.Source.clip = StageManager.Track.Clip;
            
            float playDelay = delay - Calibration.Offset + StageManager.Track.Offset * StageManager.Track.SPB;
            if (playDelay < 0f) playDelay = 0f; // 음
            owner.Source.PlayDelayed(Mathf.Repeat(playDelay, StageManager.Track.Clip.length));
            
            SLogger.Log($"Track Started: Delay={delay}, Offset={StageManager.Track.Offset}, ClipLength={owner.Source.clip.length}");
        }

        public override void OnUpdate(BeatManager owner)
        {
            if (owner.Timer.IsFinished)
            {
                Set<Gameplay>();
            }
        }
    }

    class Gameplay : State<BeatManager>
    {
        public override void OnBegin(BeatManager owner)
        {
            StageManager.Instance.Begin();
        }

        public override void OnUpdate(BeatManager owner)
        {
            BeatUtils.RoundBeat(StageManager.Track.BPM, owner.Timer.Elapsed, out int mainBeat, out int subBeat);
            owner.InputBeat = mainBeat;
            owner.InputSubBeat = subBeat;
        }

        public override void OnEnd(BeatManager owner)
        {
            owner.Source.Stop();
        }
    }

    public void End()
    {
        stateMachine.Set(null);
    }
}

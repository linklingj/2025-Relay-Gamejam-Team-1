using UnityEngine;
using SeolMJ;
using Sirenix.OdinInspector;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [field: Header("References")]
    [field: SerializeField] public AudioSource Source { get; private set; }

    // --- Input을 '약간' 앞당길 양(서브비트 단위). 0~0.49 권장 ---
    [SerializeField, Range(0f, 0.49f)]
    float inputLeadSub = 0.25f; // L=4, 120BPM 기준 약 31.25ms 선반영

    double _dspBeat0;
    double _dspAudioStart;

    public float BPS => StageManager.Track.BPS;

    bool _stopped = false;
    double _stoppedElapsedSec;
    double _stoppedDspTime; // 일시 정지한 시간 동안 누적된 DSP 시간 (이 시간은 게임 시간에 반영 x)
    double _cachedDspTime;

    public double ElapsedSec => (_stopped
        ? _stoppedElapsedSec
        : AudioSettings.dspTime - _dspBeat0 - Calibration.Offset) - _stoppedDspTime;

    // 기존 Beat/Current*는 '표시/스폰용 기준'으로 유지하되 같은 양자화 사용
    public int CurrentBeat
    {
        get { BeatUtils.Quantize(StageManager.Track.BPM, ElapsedSec, out var mb, out var sb, 0.0); return mb; }
    }
    public int CurrentSubBeat
    {
        get { BeatUtils.Quantize(StageManager.Track.BPM, ElapsedSec, out var mb, out var sb, 0.0); return sb; }
    }

    public int InputBeat { get; private set; }
    public int InputSubBeat { get; private set; }
    
    public float Beat01 => (float)((ElapsedSec / StageManager.Track.SPB) - CurrentBeat);

    FSM<BeatManager> _stateMachine;

    void Awake() => Instance = this;

    void Start()
    {
        PlayerInputs.Enable();
        _stoppedDspTime = 0;
    }

    void Update()
    {
        // if (_stopped) return;
        _stateMachine ??= new FSM<BeatManager>(this).Set<WaitForDelay>();
        // Debug.Log(ElapsedSec);

        _stateMachine?.Update();
    }

    class WaitForDelay : State<BeatManager>
    {
        public override void OnBegin(BeatManager owner)
        {
            owner._stopped = false;
            var dspNow = AudioSettings.dspTime;

            double delaySec = StageManager.Track.Delay * StageManager.Track.SPB;
            owner._dspBeat0 = dspNow + delaySec;

            double trackOffsetSec = StageManager.Track.Offset * StageManager.Track.SPB;
            owner.Source.clip = StageManager.Track.Clip;
            owner._dspAudioStart = owner._dspBeat0 + trackOffsetSec;
            owner.Source.PlayScheduled(owner._dspAudioStart);

            SLogger.Log(
                $"Track Scheduled: dspNow={dspNow:F3}, Beat0DSP={owner._dspBeat0:F3}, " +
                $"AudioStartDSP={owner._dspAudioStart:F3}, " +
                $"Delay={delaySec:F3}s, TrackOffset={StageManager.Track.Offset:F3} beats"
            );
        }

        public override void OnUpdate(BeatManager owner)
        {
            if (AudioSettings.dspTime >= owner._dspBeat0)
                Set<Gameplay>();
        }
    }

    class Gameplay : State<BeatManager>
    {
        public override void OnBegin(BeatManager owner)
        {
            StageManager.Instance.Begin();
            SLogger.Log("Beat Flow Started");
        }

        public override void OnUpdate(BeatManager owner)
        {
            // Input은 '살짝 우선시'된 그리드로 양자화
            BeatUtils.Quantize(StageManager.Track.BPM, owner.ElapsedSec,
                               out int mainBeat, out int subBeat,
                               owner.inputLeadSub, BeatUtils.SUB_BEAT_LENGTH);
            owner.InputBeat    = mainBeat;
            owner.InputSubBeat = subBeat;
        }

        public override void OnEnd(BeatManager owner)
        {
            owner.Source.Pause();
        }
    }

    public void StartBeatFlow() {
        if (!_stopped) return;

        _stoppedDspTime += AudioSettings.dspTime - _cachedDspTime;

        _stopped = false;
        Source.Play();
    }

    public void PauseBeatFlow() {
        if (_stopped) return;

        _stoppedElapsedSec = AudioSettings.dspTime - _dspBeat0 - Calibration.Offset;
        _cachedDspTime = AudioSettings.dspTime;
        _stopped = true;

        Source.Pause();
    }

    public void StopBeatFlow(bool stopaudio = true)
    {
        if (_stopped) return;
        _stoppedElapsedSec = AudioSettings.dspTime - _dspBeat0 - Calibration.Offset;
        _stopped = true;
        if (stopaudio) End();
    }

    public void End() => _stateMachine.Set(null);
}

using UnityEngine;
using SeolMJ;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [field: Header("References")]
    [field: SerializeField] public AudioSource Source { get; private set; }

    // ⬇️ dsp 기준 앵커
    double _dspBeat0;       // "비트 0"이 발생하는 DSP 시각(캘리브레이션 미포함)
    double _dspAudioStart;  // 오디오가 실제로 플레이되는 DSP 시각(로그용)

    public float BPS => StageManager.Track.BPS;

    // ⬇️ 매 프레임 Calibration.Offset을 반영 (실시간 적용)
    public double ElapsedSec => AudioSettings.dspTime - _dspBeat0 - Calibration.Offset;
    public float Beat => (float)(ElapsedSec * BPS);
    
    public int CurrentBeat => Mathf.FloorToInt(Beat);
    public int CurrentSubBeat => Mathf.FloorToInt(Beat * BeatUtils.SUB_BEAT_LENGTH) % BeatUtils.SUB_BEAT_LENGTH;

    public int InputBeat { get; private set; }
    public int InputSubBeat { get; private set; }

    FSM<BeatManager> _stateMachine;

    void Awake() => Instance = this;

    void Start()
    {
        PlayerInputs.Enable();
    }

    void Update()
    {
        _stateMachine ??= new FSM<BeatManager>(this).Set<WaitForDelay>();
        _stateMachine.Update();
    }

    class WaitForDelay : State<BeatManager>
    {
        public override void OnBegin(BeatManager owner)
        {
            // ⬇️ 모든 '절대 시간'은 DSP 기준으로 고정
            var dspNow = AudioSettings.dspTime;

            // 카운트다운(딜레이) - 초 단위
            double delaySec = StageManager.Track.Delay * StageManager.Track.SPB;

            // "비트 0"이 발생하는 DSP 시각 (곡/씬의 기준점)
            owner._dspBeat0 = dspNow + delaySec;

            // 트랙 자체 오프셋(곡 파일과 비트 그리드의 정렬 차이) - 초 단위
            double trackOffsetSec = StageManager.Track.Offset * StageManager.Track.SPB;

            // 오디오 재생은 '비트0 + 트랙오프셋'에 맞춰 스케줄 (Calibration은 스케줄에 개입 X)
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
            // 딜레이 끝나면 게임 시작
            if (AudioSettings.dspTime >= owner._dspBeat0)
                Set<Gameplay>();
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
            // ⬇️ 여기서도 Timer.Elapsed 대신 DSP 기반 경과시간 사용
            float sec = (float)owner.ElapsedSec;
            BeatUtils.RoundBeat(StageManager.Track.BPM, sec, out int mainBeat, out int subBeat);
            owner.InputBeat = mainBeat;
            owner.InputSubBeat = subBeat;
        }

        public override void OnEnd(BeatManager owner)
        {
            owner.Source.Stop();
        }
    }

    public void End() => _stateMachine.Set(null);
}

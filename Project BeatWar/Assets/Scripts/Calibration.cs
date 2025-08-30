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

    [Header("References")] public AudioSource Source;
    public float BPM;

    [Header("Settings")] public int TestCount;

    [Header("Events")] public UnityEvent OnReady;
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
            if (timeStamp == null || tapCount == 0)
            {
                SLogger.Log("No taps collected; keeping previous Offset.");
                return;
            }

            float spb = 60f / owner.BPM; // seconds per beat
            int n = tapCount; // 보통 owner.TestCount와 동일
            float[] residuals = new float[n];

            // 각 탭의 '예상 비트 시각' 대비 오차(residual) 계산
            for (int i = 0; i < n; i++)
                residuals[i] = timeStamp[i] - i * spb;

            // 중앙값 및 MAD(중앙값 절대편차) 계산
            float med = Median(residuals, n);
            float[] absdev = new float[n];
            for (int i = 0; i < n; i++)
                absdev[i] = Mathf.Abs(residuals[i] - med);

            float mad = Median(absdev, n);

            // MAD→표준편차 근사 (가우시안 가정하에 1.4826 배)
            const float MAD_TO_SIGMA = 1.4826f;
            const float K = 3f; // 3σ 임계값 (필요 시 2~3 조정)
            float sigma = mad * MAD_TO_SIGMA;

            // 이상치 제거 후 평균
            float sum = 0f;
            int kept = 0;

            if (sigma < 1e-6f)
            {
                // 분산이 거의 없으면 전부 사용(또는 중앙값 사용과 유사)
                for (int i = 0; i < n; i++)
                {
                    sum += residuals[i];
                    kept++;
                }
            }
            else
            {
                float thr = K * sigma;
                for (int i = 0; i < n; i++)
                {
                    if (Mathf.Abs(residuals[i] - med) <= thr)
                    {
                        sum += residuals[i];
                        kept++;
                    }
                }
            }

            float finalOffset = kept > 0 ? (sum / kept) : med;

            Calibration.Offset = finalOffset;
            PlayerPrefs.SetFloat("Offset", Calibration.Offset);
            SLogger.Log($"Offset: {Calibration.Offset:0.###}s (removed {n - kept}/{n} outliers)");
        }

        // 중앙값 계산 (앞쪽 count개만 고려)
        static float Median(float[] arr, int count)
        {
            float[] tmp = new float[count];
            System.Array.Copy(arr, tmp, count);
            System.Array.Sort(tmp);
            if ((count & 1) == 1) return tmp[count / 2];
            return 0.5f * (tmp[count / 2 - 1] + tmp[count / 2]);
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
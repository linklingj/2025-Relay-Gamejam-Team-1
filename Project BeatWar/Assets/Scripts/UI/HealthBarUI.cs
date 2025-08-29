using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HealthBarUI : MonoBehaviour
{
    Image _fill;

    [SerializeField] float easeDuration = 0.3f; // 애니메이션 시간
    [SerializeField] bool useUnscaledTime; // 일시정지 시에도 동작할지 여부

    float _animTime;
    float _startValue;
    float _endValue;

    void Awake()
    {
        _fill = GetComponent<Image>();
    }

    void OnEnable()
    {
        // 시작 시 현재 체력으로 초기화
        _endValue = SafeGetFillAmount();
        _startValue = _endValue;
        _animTime = 0f;
        if (_fill != null) _fill.fillAmount = _endValue;
    }

    void Update()
    {
        // 목표값 계산
        float target = SafeGetFillAmount();

        // 목표가 바뀌면 새 이징 시작
        if (!Mathf.Approximately(target, _endValue))
        {
            _startValue = _fill != null ? _fill.fillAmount : _startValue;
            _endValue = target;
            _animTime = 0f;
        }

        // 이징 진행 (OutQuad)
        if (!Mathf.Approximately((_fill != null ? _fill.fillAmount : _startValue), _endValue))
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            _animTime += dt;
            float t = easeDuration > 0f ? Mathf.Clamp01(_animTime / easeDuration) : 1f;
            float eased = EaseOutQuad(t);
            if (_fill != null)
                _fill.fillAmount = Mathf.Lerp(_startValue, _endValue, eased);
        }
    }

    // 안전한 값 계산: 플레이어나 최대 체력이 유효하지 않을 때 대비
    private float SafeGetFillAmount()
    {
        if (Player.Instance == null) return 0f;
        int maxHp = Player.Instance.MaxHealth;
        if (maxHp <= 0) return 0f;
        float v = Player.Instance.Health / (float)maxHp;
        return Mathf.Clamp01(v);
    }

    private static float EaseOutQuad(float t)
    {
        // 1 - (1 - t)^2
        float inv = 1f - t;
        return 1f - inv * inv;
    }
}

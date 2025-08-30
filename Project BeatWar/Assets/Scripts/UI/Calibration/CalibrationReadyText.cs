using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CalibrationReadyText : MonoBehaviour {
    private TextMeshProUGUI _text;
    
    [SerializeField]
    private int initialTick = 4;
    private int _currentTick;

    void Awake() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize() {
        _currentTick = initialTick;

        UpdateText();
    }

    public void Tick() {
        _currentTick--;

        UpdateText();
    }

    private void UpdateText() {
        if (!isActiveAndEnabled) return;

        _text.text = $"준비...{_currentTick}";
    }
}
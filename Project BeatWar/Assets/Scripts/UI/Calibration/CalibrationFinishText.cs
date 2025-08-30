using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CalibrationFinishText : MonoBehaviour {
    private TextMeshProUGUI _text;

    void Awake() {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void Show() {
        var offset = PlayerPrefs.GetFloat("Offset", 0f);
        
        _text.text = $"<size=1.2em>보정 완료!</size>\nOffset: {offset:0.###}s\n\n아무 키나 눌러 돌아가세요.";
    }
}
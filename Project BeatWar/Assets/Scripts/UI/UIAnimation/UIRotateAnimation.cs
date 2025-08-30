using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UIRotateAnimation : UIAnimation {
    [PropertySpace(8)]
    [TabGroup("Options")]
    [SerializeField]
    private Vector3 endValue = Vector3.zero;

    [TabGroup("Options")]
    [SerializeField]
    private RotateMode rotateMode;

    [TabGroup("Options")]
    [SerializeField]
    private bool useLocalRotation = false;

    protected override Tween CreateTween() {
        if (useLocalRotation) {
            return ((Transform)target).DOLocalRotate(endValue, duration, rotateMode);
        } else {
            return ((Transform)target).DORotate(endValue, duration, rotateMode);
        }
    }

    private void Reset() {
        OnValidate();
    }

    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<RectTransform>();
        }
    }
}
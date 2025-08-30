using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UILineAnimation : UIAnimation {
    [PropertySpace(8)]
    [TabGroup("Options")]
    [SerializeField]
    private Vector3 endValue;

    [TabGroup("Options")]
    [SerializeField]
    private bool snapping;

    protected override Tween CreateTween() {
        if (target is RectTransform rt) return rt.DOAnchorPos(endValue, duration, snapping);
        return transform.DOMove(endValue, duration, snapping);
    }

    private void Reset() {
        OnValidate();
    }
    
    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<RectTransform>();
        }

        if (target == null) {
            target = GetComponent<Transform>();
        }
        
        // options &= ~TweenOptions.IsRelative;
    }

    protected override bool IsValidTarget() {
        return base.IsValidTarget() && (target is Transform || target is RectTransform);
    }
}
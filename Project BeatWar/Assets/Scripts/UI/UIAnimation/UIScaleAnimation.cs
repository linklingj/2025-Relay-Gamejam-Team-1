using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIScaleAnimation : UIAnimation {
    [PropertySpace(8)]
    [TabGroup("Options")]
    [SerializeField]
    private Vector3 endValue;

    protected override Tween CreateTween() {
        return ((RectTransform)target).DOScale(endValue, duration);
    }

    private void Reset() {
        OnValidate();
    }
    
    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<RectTransform>();
        }
        
        options &= ~TweenOptions.IsRelative;
    }

    protected override bool IsValidTarget() {
        return base.IsValidTarget() && target is RectTransform;
    }
}
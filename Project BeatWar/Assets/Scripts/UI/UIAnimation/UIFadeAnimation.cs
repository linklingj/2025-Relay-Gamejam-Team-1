using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeAnimation : UIAnimation {
    [PropertySpace(8)]
    [TabGroup("Options")]
    [SerializeField]
    private float endValue = 1f;

    protected override Tween CreateTween() {
        if (target is CanvasGroup canvasGroup) {
            return canvasGroup.DOFade(endValue, duration);
        } else if (target is Graphic graphic) {
            return graphic.DOFade(endValue, duration);
        } else if (target is SpriteRenderer spriteRenderer) {
            return spriteRenderer.DOFade(endValue, duration);
        }

        return null;
    }

    private void Reset() {
        OnValidate();
    }

    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<Graphic>();
        }

        if (target == null) {
            target = GetComponent<CanvasGroup>();
        }

        if (target == null) {
            target = GetComponent<SpriteRenderer>();
        }
    }

    protected override bool IsValidTarget() {
        return base.IsValidTarget() && (target is CanvasGroup || target is Graphic || target is SpriteRenderer);
    }
}
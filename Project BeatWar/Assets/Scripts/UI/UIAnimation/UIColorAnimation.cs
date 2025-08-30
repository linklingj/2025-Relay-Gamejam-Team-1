using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIColorAnimation : UIAnimation {
    [SerializeField] 
    private Color targetColor = Color.white;

    protected override Tween CreateTween() {
        var graphic = GetComponent<Graphic>();

        return graphic.DOColor(targetColor, duration);
    }

    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<Graphic>();
        }
    }

    protected override bool IsValidTarget() {
        return base.IsValidTarget() && target is Graphic;
    }
}
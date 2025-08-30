using System;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public abstract class UIFloatAnimation : UIAnimation {
    [SerializeField]
    private float endValue;

    protected abstract float Get();
    protected abstract void Set(float value);

    protected sealed override Tween CreateTween() {
        return DOTween.To(() => Get(), x => Set(x), endValue, duration);
    }

    protected override void OnValidate() {
        base.OnValidate();

        target = this;
    }
}
#pragma warning disable IDE1006

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Flags] 
public enum TweenOptions {
    IsRelative = 1 << 0,
    IsFrom     = 1 << 1,
    AutoKill   = 1 << 2,
    AutoPlay   = 1 << 3
}

// [RequireComponent(typeof(RectTransform))]
public abstract class UIAnimation : ComponentGroupMember<UIAnimationGroup, UIAnimation>, IAnimation {
    [SerializeField, DisableInPlayMode]
    [PropertyOrder(-1)]
    [Required, ValidateInput(nameof(IsValidTarget))]
    protected Component target;

    [TabGroup("Playback")]
    public float duration = 1f;

    [TabGroup("Playback")]
    public float delay;

    [TabGroup("Playback")]
    public Ease ease = Ease.Linear;

    [PropertySpace(8)]
    [TabGroup("Playback")]
    [MinValue(-1)]
    public int loops = 1;

    [TabGroup("Playback")]
    [ShowIf("@loops == -1 || loops >= 2")]
    public LoopType loopType;

    [TabGroup("Options")]
    [EnumToggleButtons, HideLabel]
    public TweenOptions options = TweenOptions.AutoKill;

    [TabGroup("Options")]
    public bool ignoreTimeScale = true;

    [TabGroup("Options")]
    [ShowIf("@options.HasFlag(TweenOptions.AutoPlay)")]
    public bool playOnEnable = false;

    [TabGroup("Options")]
    public bool initializeOnAwake = true;

    [FoldoutGroup("Events")]
    public UnityEvent onPlay;

    [FoldoutGroup("Events")]
    public UnityEvent onPause;

    [FoldoutGroup("Events")]
    public UnityEvent onComplete;
    
    protected bool isRelative => (options & TweenOptions.IsRelative) != 0;
    protected bool isFrom => (options & TweenOptions.IsFrom) != 0;
    protected bool autoKill => (options & TweenOptions.AutoKill) != 0;
    protected bool autoPlay => (options & TweenOptions.AutoPlay) != 0;

    protected Tween tween;

    public bool IsInitialized => tween != null && tween.IsActive();
    public bool IsPlaying => tween != null && tween.IsActive() && tween.IsPlaying();

    protected override void Awake() {
        base.Awake();

        if (initializeOnAwake) {
            Initialize();
        }
    }

    protected virtual void OnEnable() {
        if (options.HasFlag(TweenOptions.AutoPlay) && playOnEnable) {
            Play();
        }
    }

    protected virtual void OnDisable() {
        if (options.HasFlag(TweenOptions.AutoPlay) && playOnEnable) {
            Pause();
            tween?.Rewind();
        } else if (options.HasFlag(TweenOptions.AutoKill)) {
            Kill();
        } 
    }

    protected abstract Tween CreateTween();

    protected virtual void Initialize() {
        Kill();
        tween = CreateTween();

        if (tween == null) {
            return;
        }

        if (isFrom) {
            ((Tweener)tween).From(isRelative);
        } else {
            tween.SetRelative(isRelative);
        }

        tween.SetTarget(target.gameObject)
            .SetDelay(delay)
            .SetLoops(loops, loopType)
            .SetAutoKill(autoKill)
            .SetUpdate(ignoreTimeScale)
            .OnKill(() => tween = null);

        tween.SetEase(ease);

        if (autoPlay) {
            tween.Play();
        } else {
            tween.Pause();
        }

        tween.OnPlay(onPlay.Invoke);
        tween.OnPause(onPause.Invoke);
        tween.OnComplete(onComplete.Invoke);
    }

    [PropertySpace(8)]
    [ButtonGroup("Buttons", VisibleIf = "@UnityEngine.Application.isPlaying")]
    [Button("Play", ButtonSizes.Large)]
    public virtual void Play() {
        if(!IsInitialized) {
            Initialize();
        }
        
        if (tween != null && !tween.IsPlaying()) {
            tween.PlayForward();
            // tween.Play();
        }
    }
    
    [ButtonGroup("Buttons")]
    [Button("Pause", ButtonSizes.Large)]
    public virtual void Pause() {
        if (IsInitialized && tween.IsPlaying()) {
            tween.Pause();
        }
    }

    [ButtonGroup("Buttons")]
    [Button("Complete", ButtonSizes.Large)]
    public virtual void Complete() {
        if (IsInitialized) {
            tween.Complete();
        }
    }

    [ButtonGroup("Buttons")]
    [Button("Kill", ButtonSizes.Large)]
    public virtual void Kill(bool complete = true) {
        if (IsInitialized) {
            tween.Kill(complete);
            tween = null;
        }
    }

    [ButtonGroup("Buttons")]
    [Button("Rewind", ButtonSizes.Large)]
    public virtual void Rewind() {
        if (IsInitialized) {
            tween.Rewind();
        }
    }

    public virtual void SmoothRewind() {
        if (IsInitialized) {
            tween.SmoothRewind();
        }
    }

    protected virtual bool IsValidTarget() => target != null;

    protected override void OnDestroy() {
        base.OnDestroy();

        Complete();
        Kill();
    }
}
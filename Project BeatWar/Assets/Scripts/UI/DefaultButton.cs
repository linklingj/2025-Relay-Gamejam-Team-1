using UnityEngine;

public class DefaultButton : MonoBehaviour {
    [SerializeField]
    private UIAnimationGroup _disableAnimation;
    private void OnDisable() {
        if (!Application.isPlaying) return; // 게임 종료 시에 오류 방지

        _disableAnimation.Play();
        _disableAnimation.Complete();
    }
}

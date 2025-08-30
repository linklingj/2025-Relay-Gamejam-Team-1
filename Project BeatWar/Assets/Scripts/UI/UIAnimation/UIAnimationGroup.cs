using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIAnimationGroup : HashableComponentGroup<UIAnimationGroup, UIAnimation>, IAnimation {
    [ShowIf("@UnityEngine.Application.isPlaying"), ShowInInspector]
    public bool IsPlaying {
        get {
            return childrenGroups.Any(child => child != null && child.IsPlaying) ||
                   members.Any(animation => animation != null && animation.IsPlaying);
        }
    }

    [PropertySpace(8)]
    [ButtonGroup("Buttons", VisibleIf = "@UnityEngine.Application.isPlaying")]
    [Button("Play", ButtonSizes.Large)]
    public void Play() {
        if (IsPlaying) {
            return;
        }

        childrenGroups.ForEach(child => child.Play());
        members.ForEach(animation => animation.Play());
    }
    
    [ButtonGroup("Buttons")]
    [Button("Pause", ButtonSizes.Large)]
    public void Pause() {
        childrenGroups.ForEach(child => child.Pause());
        members.ForEach(animation => animation.Pause());
    }

    [ButtonGroup("Buttons")]
    [Button("Complete", ButtonSizes.Large)]
    public void Complete() {
        childrenGroups.ForEach(child => child.Complete());
        members.ForEach(animation => animation.Complete());
    }

    [ButtonGroup("Buttons")]
    [Button("Kill", ButtonSizes.Large)]
    public void Kill(bool complete = true) {
        childrenGroups.ForEach(child => child.Kill(complete));
        members.ForEach(animation => animation.Kill(complete));
    }

    [ButtonGroup("Buttons")]
    [Button("Rewind", ButtonSizes.Large)]
    public void Rewind() {
        childrenGroups.ForEach(child => child.Rewind());
        members.ForEach(animation => animation.Rewind());
    }

#if UNITY_EDITOR
    [PropertySpace(8)]
    [TabGroup("Group Settings")]
    [Button("Show Group Hierarchy", ButtonSizes.Large)]
    public void ShowHierarchyWindow() {
        var window = UnityEditor.EditorWindow.GetWindow<UIAnimationGroupEditor>();
        window.titleContent = new GUIContent("UI Animation Hierarchy");
        window.Initialize(this);
        window.Show();
    }
#endif
}

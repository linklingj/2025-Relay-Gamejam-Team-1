#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class UIAnimationGroupEditor : OdinMenuEditorWindow {
    private UIAnimationGroup _root;

    private List<UIAnimationGroup> _groups;
    private List<UIAnimation> _members;
    
    private PropertyTree _mainTree = null;
    private PropertyTree _selectionTree = null;
    private List<UIAnimation> _selectedAnimations;

    ////////////////////////////////////////////////////////////////
    
    [TabGroup("Playback")]
    public float duration;

    [TabGroup("Playback")]
    public float delay;

    [TabGroup("Playback")]
    public Ease ease;

    [PropertySpace(8)]
    [TabGroup("Playback")]
    [MinValue(-1)]
    public int loops;

    [TabGroup("Playback")]
    [ShowIf("@loops == -1 || loops >= 2")]
    public LoopType loopType;

    [TabGroup("Options")]
    [EnumToggleButtons, HideLabel]
    public TweenOptions options;

    [TabGroup("Options")]
    public bool ignoreTimeScale;

    //////////////////////////////////////////////////////////////////


    protected override void OnEnable() {
        base.OnEnable();

        _mainTree?.Dispose();
        _mainTree = PropertyTree.Create(this);
    }

    public void Initialize(UIAnimationGroup group) {
        var current = group;

        while (current.GetParent() != null) {
            current = current.GetParent();
        }

        _root = current;
        ForceMenuTreeRebuild();
    }

    protected override void DrawEditor(int index)
    {
        var selectedValue = MenuTree.Selection[index].Value;
        if (selectedValue is Component component && component != null) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Target", component, typeof(Component), false);
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(4);
        }
        
        base.DrawEditor(index);
    }

    protected override void DrawEditors() {
        var selection = MenuTree.Selection;

        if (selection.Count <= 1) {
            base.DrawEditors();
            return;
        }

        if (_selectionTree != null) {
            _selectionTree.Draw();
            return;
        }

        if (_selectedAnimations == null) return;

        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Edit Multiple UIAnimations");

        bool mixedDuration = _selectedAnimations.Select(a => a.duration).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedDuration;
        EditorGUI.BeginChangeCheck();
        float newDuration = EditorGUILayout.FloatField("Duration", _selectedAnimations[0].duration);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Duration");
                a.duration = newDuration;
                EditorUtility.SetDirty(a);
            }
        }

        bool mixedDelay = _selectedAnimations.Select(a => a.delay).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedDelay;
        EditorGUI.BeginChangeCheck();
        float newDelay = EditorGUILayout.FloatField("Delay", _selectedAnimations[0].delay);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Delay");
                a.delay = newDelay;
                EditorUtility.SetDirty(a);
            }
        }

        bool mixedEase = _selectedAnimations.Select(a => a.ease).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedEase;
        EditorGUI.BeginChangeCheck();
        Ease newEase = (Ease)EditorGUILayout.EnumPopup("Ease", _selectedAnimations[0].ease);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Ease");
                a.ease = newEase;
                EditorUtility.SetDirty(a);
            }
        }

        bool mixedLoops = _selectedAnimations.Select(a => a.loops).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedLoops;
        EditorGUI.BeginChangeCheck();
        int newLoops = EditorGUILayout.IntField("Loops", _selectedAnimations[0].loops);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Loops");
                a.loops = newLoops;
                EditorUtility.SetDirty(a);
            }
        }

        bool mixedLoopType = _selectedAnimations.Select(a => a.loopType).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedLoopType;
        EditorGUI.BeginChangeCheck();
        LoopType newLoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", _selectedAnimations[0].loopType);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Loop Type");
                a.loopType = newLoopType;
                EditorUtility.SetDirty(a);
            }
        }

        bool mixedOptions = _selectedAnimations.Select(a => a.options).Distinct().Count() > 1;
        EditorGUI.showMixedValue = mixedOptions;
        EditorGUI.BeginChangeCheck();
        TweenOptions newOptions = (TweenOptions)EditorGUILayout.EnumFlagsField("Options", _selectedAnimations[0].options);
        if (EditorGUI.EndChangeCheck()) {
            foreach (var a in _selectedAnimations) {
                Undo.RecordObject(a, "Change Animation Options");
                a.options = newOptions;
                EditorUtility.SetDirty(a);
            }
        }

        EditorGUI.showMixedValue = false;
        Undo.CollapseUndoOperations(undoGroup);
    }

    private void OnSelectionChanged(SelectionChangedType type)
    {
        var selection = MenuTree.Selection;
        
        if (selection.Count <= 1) {
            _selectionTree = null;
            return;
        }

        var allTypes = selection.Select(x => x.Value?.GetType()).Distinct().ToList();
        if (allTypes.Count == 1 && allTypes[0] != null) {
            var targets = selection.Select(x => x.Value as UnityEngine.Object).Where(x => x != null).ToArray();

            _selectionTree = PropertyTree.Create(targets);
            return;
        } 

        _selectionTree = null;

        _selectedAnimations = selection
            .Select(x => x.Value as UIAnimation)
            .Where(x => x != null)
            .ToList();
    }

    protected override OdinMenuTree BuildMenuTree() {
        var tree = new OdinMenuTree(true);
        
        tree.Config.DefaultMenuStyle.Height = 24;
        tree.Selection.SelectionChanged += OnSelectionChanged;

        if (_root != null) {
            _groups = _root.GetComponentsInChildren<UIAnimationGroup>().ToList();
            _members = _root.GetComponentsInChildren<UIAnimation>().ToList();

            var item = new OdinMenuItem(tree, _root.GetGroupName(), _root) {
                DefaultToggledState = false
            };

            AddChildGroups(tree, _root, item);

            tree.MenuItems.Add(item);
        }

        return tree;
    }
    
    private void AddChildGroups(OdinMenuTree tree, UIAnimationGroup group, OdinMenuItem parent) {
        var groups = _groups.Where(g => g.GetParent() == group).ToList();
        var members = _members.Where(m => m.GetParent() == group).ToList();

        foreach (var m in members) {
            var name = Regex.Match(m.GetType().Name, "^UI(.*)Animation$").Groups[1].Value;
            var item = new OdinMenuItem(tree, m.name + " - " + name, m) {
                DefaultToggledState = false
            };

            parent.ChildMenuItems.Add(item);
        }

        foreach (var g in groups) {
            var item = new OdinMenuItem(tree, g.GetGroupName(), g) {
                DefaultToggledState = false
            };
            parent.ChildMenuItems.Add(item);

            AddChildGroups(tree, g, item);
        }
    }
}

#endif
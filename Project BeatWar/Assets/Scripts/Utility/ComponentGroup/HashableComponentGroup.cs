using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class HashableComponentGroup<TGroup, TMember> : ComponentGroup<TGroup, TMember> 
    where TGroup : HashableComponentGroup<TGroup, TMember>
    where TMember : Component
{
    [SerializeField]
    [TabGroup("Group Settings"), DisableInPlayMode]
    private bool _useKey = false;

    [SerializeField]
    [TabGroup("Group Settings")]
    [EnableIf(nameof(_useKey))]
    private string _key;

    private readonly static Dictionary<string, TGroup> _groups = new();

    public static TGroup Get(string key) {
        if (string.IsNullOrEmpty(key)) {
            return null;
        }

        if (_groups.TryGetValue(key, out var group)) {
            if (group == null) _groups.Remove(key);

            return group;
        }

        Debug.LogError($"HashableComponentGroup with key '{key}' not found.");
        return null;
    }

    protected override void Awake() {
        base.Awake();

        if(_useKey && !string.IsNullOrEmpty(_key)) {
            _groups[_key] = this as TGroup;
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();

        if (_useKey && _groups.ContainsKey(_key)) {
            _groups.Remove(_key);
        }
    }

    public override string GetGroupName() {
        return _useKey ? _key : base.GetGroupName();
    }
}
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ComponentGroup<TGroup, TMember> : ComponentGroupMember<TGroup, TMember> 
    where TGroup : ComponentGroup<TGroup, TMember> // use CRTP 
    where TMember : Component 
{
    [TabGroup("Group Settings")]
    [ShowIf("@UnityEngine.Application.isPlaying && childrenGroups.Count > 0")]
    [SerializeField]
    protected List<TGroup> childrenGroups = new();

    [TabGroup("Group Settings")]
    [ShowIf("@UnityEngine.Application.isPlaying && members.Count > 0")]
    [SerializeField, ReadOnly, ListDrawerSettings(IsReadOnly = true)]
    protected List<TMember> members = new();

    protected override Transform iter => base.iter.parent;

    protected override void Awake() {
        group?.AddChildGroup(this as TGroup);
    }

    public void AddChildGroup(TGroup component) {
        if (childrenGroups.Contains(component)) {
            return;
        }

        childrenGroups.Add(component);
    }

    public void RemoveChildGroup(TGroup component) {
        if (!childrenGroups.Contains(component)) {
            return;
        }

        childrenGroups.Remove(component);
    }

    public void Register(TMember member) {
        if (IsGroupMember(member)) {
            return;
        }

        members.Add(member);
    }

    public void Unregister(TMember member) {
        if (!IsGroupMember(member)) {
            return;
        }

        members.Remove(member);
    }

    public bool IsGroupMember(TMember member) {
        return members.Contains(member);
    }

    public virtual string GetGroupName() {
        return $"({name})";
    }
}
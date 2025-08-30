#pragma warning disable IDE1006

using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

// member -> group register
public abstract class ComponentGroupMember<TGroup, TMember> : MonoBehaviour
    where TGroup : ComponentGroup<TGroup, TMember>
    where TMember : Component
{
    [TabGroup("Group Settings")]
    [SerializeField, DisableInPlayMode]
    [ValueDropdown(nameof(GetParentGroups))]
    protected TGroup group;

    protected virtual Transform iter => transform;

    protected virtual void Awake() {
        group?.Register(this as TMember);
    }

    public TGroup GetParent() {
        return group;
    }

    protected virtual void OnDestroy() {
        Unregister();
    }

    public void Register() {
        if (group == null && transform.parent != null) {
            group = transform.parent.GetComponentInParent<TGroup>();
        }

        group?.Register(this as TMember);
    }

    public void Unregister() {
        if (IsRegistered()) {
            group.Unregister(this as TMember);
        }
    }

    private bool IsRegistered() {
        return group != null && group.IsGroupMember(this as TMember);
    }

    protected virtual ValueDropdownList<TGroup> GetParentGroups() {
        var result = new ValueDropdownList<TGroup>();
        var iter = this.iter;

        while(iter != null) {
            var groups = iter.GetComponents<TGroup>();

            if (groups.Length == 0) {
                iter = iter.parent;
                continue;
            }

            result.AddRange(groups.Select(g => new ValueDropdownItem<TGroup>(g.GetGroupName(), g)));
            return result;
        }

        return null;
    }

    protected virtual void OnValidate() {
        if (group != null && !transform.IsChildOf(group.transform)) {
            group = null;
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeWeightAnimation : UIFloatAnimation {
    protected override float Get() {
        return (target as Volume).weight;
    }

    protected override void Set(float value) {
        (target as Volume).weight = value;
    }

    protected override void OnValidate() {
        if (target == null) {
            target = GetComponent<Volume>();
        }
    }

    protected override bool IsValidTarget() {
        return base.IsValidTarget() && target is Volume;
    }
}
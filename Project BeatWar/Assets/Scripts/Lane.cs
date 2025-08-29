using UnityEngine;
using UnityEngine.Serialization;

public class Lane : MonoBehaviour
{
    public Pattern Pattern { get; private set; }
    public int Index { get; private set; }

    public Weapon Weapon { get; private set; }
    [FormerlySerializedAs("spriteAnimator")] public CharacterAnimator characterAnimator;
    
    [SerializeField] ParticleEmitCaller onHitCaller;
    [SerializeField] ParticleEmitCaller onMissCaller;

    public void Init(int index)
    {
        Index = index;
        transform.localPosition = new Vector2(StageManager.Instance.LaneToPosition(index), 0f);
    }

    public void Refresh(Pattern pattern)
    {
        characterAnimator.ShootAnimation();
        onHitCaller.EmitAt(transform.position);
        Pattern = pattern;

        if (Weapon != null)
        {
            WeaponManager.Instance.Return(Weapon);
        }
        Weapon = WeaponManager.Instance.Borrow(pattern, this);
    }

    public void Miss()
    {
        onMissCaller.EmitAt(transform.position);
    }

    public void Clear()
    {
        Pattern = Pattern.None;

        Weapon = null;
    }
}

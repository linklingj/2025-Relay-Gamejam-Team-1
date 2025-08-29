using SeolMJ;
using UnityEngine;
using UnityEngine.Serialization;

public class Lane : MonoBehaviour
{
    public Pattern Pattern { get; private set; }
    public int Index { get; private set; }

    public Weapon Weapon { get; private set; }
    public CharacterAnimator characterAnimator;
    
    [SerializeField] ParticleEmitCaller onHitCaller;
    [SerializeField] ParticleEmitCaller onMissCaller;
    public float missRandomRange = 0.5f;

    public void Init(int index)
    {
        Index = index;
        transform.localPosition = new Vector2(StageManager.Instance.LaneToPosition(index), 0f);
    }

    public void Refresh(Pattern pattern)
    {
        characterAnimator.ShootAnimation();
        
        var entity = Entity.Scan(Index, 0);
        if(entity != null) onHitCaller.EmitAt( entity.transform.position);
        
        Pattern = pattern;

        if (Weapon != null)
        {
            WeaponManager.Instance.Return(Weapon);
        }
        Weapon = WeaponManager.Instance.Borrow(pattern, this);  
    }

    public void Miss()
    {
        onMissCaller.EmitAt(new Vector2(transform.position.x + Random.Range(-missRandomRange, missRandomRange), Random.Range(-missRandomRange, missRandomRange)));
    }

    public void Clear()
    {
        Pattern = Pattern.None;

        Weapon = null;
    }
}

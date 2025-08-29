using UnityEngine;

public class Enemy : Entity
{
    public Enemy Original { get; private set; }

    public Pattern Weakness { get; private set; }

    public virtual void Enable(Enemy original)
    {
        Original = original;
        Weakness = PatternUtils.Random(Health);
        Beat = BeatManager.Instance.CurrentBeat;
        base.Enable();
    }

    protected override void Update()
    {
        base.Update();
        // 적 못잡았을 때 검사
        if (transform.position.y <= 0f)
        {
            Disable();
            Player.Instance.Deal(Health);
            EnemyManager.Instance.OnHitCaller.EmitAt(Player.Instance.transform.position);
        }
    }

    public override void Disable()
    {
        base.Disable();
    }
}

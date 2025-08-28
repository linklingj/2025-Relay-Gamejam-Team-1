using UnityEngine;

public class Lane : MonoBehaviour
{
    public Pattern Pattern { get; private set; }
    public int Index { get; private set; }

    public Weapon Weapon { get; private set; }

    public void Init(int index)
    {
        Index = index;
        transform.localPosition = new Vector2(StageManager.Instance.LaneToPosition(index), 0f);
    }

    public void Refresh(Pattern pattern)
    {
        Pattern = pattern;

        if (Weapon != null)
        {
            WeaponManager.Instance.Return(Weapon);
        }
        Weapon = WeaponManager.Instance.Borrow(pattern, this);
    }

    public void Clear()
    {
        Pattern = Pattern.None;

        Weapon = null;
    }
}

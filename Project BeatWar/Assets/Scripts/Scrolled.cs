using UnityEngine;

public abstract class Scrolled : MonoBehaviour
{
    public Vector2 Position
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }

    public float PositionX
    {
        get => Position.x;
        set => Position = new Vector2(value, Position.y);
    }

    public float PositionY
    {
        get => Position.y;
        set => Position = new Vector2(Position.x, value);
    }

    public int Beat;

    protected virtual void Update()
    {
        PositionY = (Beat - BeatManager.Instance.CurrentBeat - BeatManager.Instance.CurrentSubBeat / (float)BeatUtils.SUB_BEAT_LENGTH) * StageManager.Instance.ScrollSpeed;
    }
}

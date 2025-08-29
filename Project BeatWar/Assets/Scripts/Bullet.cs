using UnityEngine;

public class Bullet : MonoBehaviour
{
    Weapon weapon;

    void Awake()
    {
        weapon = GetComponentInParent<Weapon>();
    }

    void OnEnable()
    {
        transform.localPosition = Vector2.zero;
    }

    void Update()
    {
        float progress = Mathf.Max(BeatManager.Instance.Beat - weapon.Beat, 0f);
        Entity target = Entity.Scan(weapon.Lane.Index, weapon.Damage);
        if (target != null)
        {
            transform.position = Vector2.Lerp(transform.parent.position, target.transform.position, progress);
        }
        else
        {
            transform.position = Vector2.LerpUnclamped(transform.parent.position, new Vector2(transform.parent.position.x, 0.75f), progress);
        }
    }

    float EaseSquare(float x)
    {
        return x * x;
    }
}

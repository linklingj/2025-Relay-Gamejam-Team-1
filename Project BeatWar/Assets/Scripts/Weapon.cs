using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public static readonly List<Weapon> Instances = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnLoad()
    {
        Instances.Clear();
    }

    [field: Header("Settings")]
    [field: SerializeField]
    public int Damage { get; private set; }
    public Pattern Pattern { get; private set; }

    [SerializeField]
    UnityEvent<Entity> onAttack;
    [SerializeField]
    UnityEvent<Vector2> onAttackPosition;

    [SerializeField] private Lane _lane;
    public Lane Lane { get => _lane; private set => _lane = value; }
    public int Beat { get; private set; }

    public void Enable(Pattern pattern, Lane lane)
    {
        Lane = lane;
        Pattern = pattern;
        transform.localPosition = new Vector2(StageManager.Instance.LaneToPosition(lane.Index), 0f);

        Beat = BeatManager.Instance.InputBeat;
        Instances.Add(this);
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        Instances.Remove(this);
    }

    public void Attack()
    {
        Entity target = Entity.Scan(Lane.Index, Damage);
        if (target != null && target.Health <= Damage)
        {
            target.Kill();
            if (target is Enemy enemy && enemy.Weakness == Pattern)
            {
                // Break!
                Player.Instance.Heal(Damage);
                ScoreSystem.AddScore(ScoreSystem.Judge.Perfect);
            }
            else ScoreSystem.AddScore(ScoreSystem.Judge.Good);
            
            onAttack.Invoke(target);
            onAttackPosition.Invoke(target.transform.position);
        }
        else
        {
            Player.Instance.Deal(Damage);
            EnemyManager.Instance.OnHitCaller.EmitAt(Lane.transform.position);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Constants

    public static readonly Key[] Keys = new Key[]
    {
        Key.A, Key.S, Key.D, Key.F,
        Key.J, Key.K, Key.L, Key.Semicolon
    };

    #endregion

    public static Player Instance;

    [field: Header("Settings")]
    [field: SerializeField]
    public int MaxHealth { get; private set; }

    public int Health { get; private set; }

    public ParticleEmitCaller OnHealCaller { get; private set; }

    public void Deal(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            StageManager.Instance.End();
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        OnHealCaller.EmitAt(transform.position);
    }

    [Header("References")]
    [SerializeField]
    Lane[] lanes;

    void StartLanes()
    {
        for (int i = 0; i < 8; i++)
        {
            lanes[i].Init(i);
            lanes[i].gameObject.SetActive(StageManager.Track.Lanes.HasLane(i));
        }
    }

    void ClearLanes()
    {
        for (int i = 0; i < 8; i++)
        {
            lanes[i].Clear();
        }
    }

    void Awake()
    {
        Instance = this;
        Health = MaxHealth;
        OnHealCaller = GetComponent<ParticleEmitCaller>();
    }

    int beat;

    void Start()
    {
        beat = BeatManager.Instance.InputBeat;
        StartLanes();
    }

    void Update()
    {
        if (BeatManager.Instance.InputBeat >= 0
            && beat != BeatManager.Instance.InputBeat)
        {
            beat = BeatManager.Instance.InputBeat;
            ClearLanes();
        }

        for (int i = 0; i < 8; i++)
        {
            if (StageManager.Track.Lanes.HasLane(i))
            {
                var key = Keys[i];
                var control = Keyboard.current[key];

                if (control.wasPressedThisFrame)
                {
                    Lane lane = lanes[i];

                    int subBeat = BeatManager.Instance.InputSubBeat;
                    if (!lane.Pattern.HasPattern(subBeat))
                    {
                        lane.Refresh(lane.Pattern
                            | subBeat.ToPattern());
                    }
                    else
                    {
                        lane.Miss();
                    }
                }
            }
        }
    }
}


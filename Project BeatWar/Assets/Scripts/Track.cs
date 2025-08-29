using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Chart", menuName = "Chart")]
public class Track : ScriptableObject
{
    [field: Header("Track Settings")]
    /// <summary>
    /// Name of this track. Usually the name of the song
    /// </summary>
    [field: SerializeField]
    public string Name { get; private set; }

    /// <summary>
    /// Beats per Minute
    /// </summary>
    [field: SerializeField]
    public float BPM { get; private set; }

    /// <summary>
    /// Beats per Second
    /// </summary>
    public float BPS => BPM / 60f;

    /// <summary>
    /// Seconds per Beat
    /// </summary>
    public float SPB => 60f / BPM;

    /// <summary>
    /// Delays the entire gameplay by this (beats)
    /// </summary>
    [field: SerializeField]
    public float Delay { get; private set; }

    /// <summary>
    /// Delays the music playback by this (beats)
    /// </summary>
    [field: SerializeField]
    public float Offset { get; private set; }

    [field: SerializeField, EnumButtons]
    public Pattern[] Chart { get; set; }

    /// <summary>
    /// Song to play for the track
    /// </summary>
    [field: SerializeField]
    public AudioClip Clip { get; private set; }

    [field: SerializeField, EnumButtons]
    public Lanes Lanes { get; private set; }

    [field: Header("Enemy Settings")]
    /// <summary>
    /// 0~1
    /// </summary>
    [field: SerializeField]
    public float SpawnRateBase { get; private set; }

    /// <summary>
    /// SpawnRate addition per Beat.
    /// </summary>
    [field: SerializeField]
    public float SpawnRateAddition { get; private set; }
    
    [field: Header("Chart Settings")]
    public int patternStartBeat = 0; // the beat at which the pattern starts
    public int Interval = 2; // in beats
    public int enemiesPerSpawn = 1; // enemies spawned per spawn event
    public bool randomizeLanes = true;

    /// <summary>
    /// SpawnRate calculated with Tanh(SpawnRateBase + (SpawnRateAddition * CurrentBeat));
    /// ex)
    ///     SpawnRateBase:0.2
    ///     SpawnRateAddition:0.05
    ///     CurrentBeat:20
    ///     => Tanh(0.2+1.0) => approximately 0.83
    /// </summary>
    /// <param name="beat">current beat (corresponds to elapsed time)</param>
    /// <returns>0~1</returns>
    public float SpawnRate(float beat) => System.MathF.Tanh(SpawnRateBase + SpawnRateAddition * beat);
    
    
    #region Meta Data
    [field: FormerlySerializedAs("<Author>k__BackingField")]
    [field: Header("Meta Data")]
    [field: SerializeField]
    public string Artist { get; private set; }
    
    [field: SerializeField]
    public int Difficulty { get; private set; }
    
    [field: SerializeField]
    public Sprite Cover { get; private set; }
    
    [field: SerializeField]
    public float PreviewStart { get; private set; }
    
    
    #endregion
}

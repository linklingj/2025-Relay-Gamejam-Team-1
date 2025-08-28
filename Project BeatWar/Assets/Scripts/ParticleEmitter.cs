using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    static readonly Dictionary<string, ParticleEmitter> emitters = new();

    public static ParticleEmitter Get(string name)
    {
        return emitters[name];
    }

    ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        emitters.Add(gameObject.name, this);
    }

    public void Emit(Vector2 position, float angle, Vector2 scale, Color color, float lifetime)
    {
        ParticleSystem.EmitParams param = new()
        {
            position = position,
            rotation = -angle,
            startSize3D = new(particle.main.startSizeX.Evaluate(0) * scale.x, particle.main.startSizeY.Evaluate(0) * scale.y, 1f),
            startColor = color,
            startLifetime = lifetime,
        };
        particle.Emit(param, 1);
    }

    public void Emit()
    {
        particle.Emit(1);
    }

    void OnDestroy()
    {
        emitters.Remove(gameObject.name);
    }
}

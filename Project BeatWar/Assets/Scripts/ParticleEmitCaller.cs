using UnityEngine;

public class ParticleEmitCaller : MonoBehaviour
{
    [SerializeField] string emitterName;
    [SerializeField] Vector2 position;
    [SerializeField] float angle;
    [SerializeField] Vector2 scale;
    [SerializeField] Color color;
    [SerializeField] float lifetime;

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public void Emit()
    {
        ParticleEmitter.Get(emitterName).Emit(position, angle, scale, color, lifetime);
    }

    public void EmitAt(Vector2 position)
    {
        this.position = position;
        Emit();
    }
}

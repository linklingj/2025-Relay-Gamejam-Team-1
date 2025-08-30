public interface IAnimation { 
    public bool IsPlaying { get; }
    public void Play();
    public void Pause();
    public void Complete();
    public void Kill(bool complete = true);
    public void Rewind();
}
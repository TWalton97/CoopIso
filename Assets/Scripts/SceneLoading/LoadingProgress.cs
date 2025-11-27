using System;

public class LoadingProgress : IProgress<float>
{
    public event Action<float> Progressed;

    const float ratio = 1f;

    public void Report(float value)
    {
        Progressed?.Invoke(obj: value / ratio);
    }
}

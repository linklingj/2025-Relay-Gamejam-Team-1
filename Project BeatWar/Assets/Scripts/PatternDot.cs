using UnityEngine;

public class PatternDot : MonoBehaviour
{
    [SerializeField]
    GameObject dot;

    public void Set(bool on)
    {
        dot.SetActive(on);
    }
}

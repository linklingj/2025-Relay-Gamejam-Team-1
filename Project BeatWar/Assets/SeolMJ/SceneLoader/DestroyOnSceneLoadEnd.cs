using SeolMJ;
using UnityEngine;

public class DestroyOnSceneLoadEnd : MonoBehaviour
{
    void Awake()
    {
        SceneLoader.OnLoadEnd += OnEnd;
    }

    public void OnEnd()
    {
        Destroy(gameObject);
    }
}

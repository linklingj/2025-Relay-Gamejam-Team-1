using SeolMJ;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public int[] Scenes;

    public void Load()
    {
        SceneLoader.Load(Scenes);
    }

    public void Load(bool reload = false)
    {
        SceneLoader.Load(Scenes, new() { Reload = reload });
    }
}

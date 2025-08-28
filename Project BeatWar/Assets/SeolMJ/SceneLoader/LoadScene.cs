using SeolMJ;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public int[] Scenes;

    public void Load()
    {
        SceneLoader.Load(Scenes);
    }
}

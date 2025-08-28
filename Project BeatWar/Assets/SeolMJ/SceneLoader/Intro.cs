using UnityEngine;

namespace SeolMJ
{
    public class Intro : MonoBehaviour
    {
        void Start()
        {
            if (!SceneLoader.IsLoading)
            {
                SceneLoader.Load(ScenePresets.MainMenu, new()
                {
                    DontSpawnPrefab = true,
                });
            }
        }
    }
}
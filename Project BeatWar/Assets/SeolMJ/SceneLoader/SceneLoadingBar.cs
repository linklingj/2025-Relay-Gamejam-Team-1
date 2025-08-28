using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeolMJ
{
    public class SceneLoadingBar : MonoBehaviour
    {
        void Update()
        {
            var size = (transform as RectTransform).sizeDelta;
            size.x = (transform.parent as RectTransform).rect.width * SceneLoader.Progress;
            (transform as RectTransform).sizeDelta = size;
        }
    }
}
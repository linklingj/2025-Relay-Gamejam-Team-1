using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraAspectRatio : MonoBehaviour {
    [SerializeField]
    private Vector2Int targetResolution = new(1920, 1080);
    private Camera _cam;

    void Start() {
        _cam = GetComponent<Camera>();

        UpdateAspectRatio();
    }
    
    [PropertySpace(8)]
    [Button("Update Aspect Ratio", ButtonSizes.Large)]
    void UpdateAspectRatio() {
        Vector2 screenSize = GetScreenSize();

        float targetRatio = (float)targetResolution.x / targetResolution.y;
        float currentRatio = screenSize.x / screenSize.y;

        if (currentRatio > targetRatio) {
            float newWidth = targetRatio / currentRatio;
            _cam.rect = new Rect((1.0f - newWidth) / 2.0f, 0, newWidth, 1.0f);
        } else {
            float newHeight = currentRatio / targetRatio;
            _cam.rect = new Rect(0, (1.0f - newHeight) / 2.0f, 1.0f, newHeight);
        }

        var data = _cam.GetUniversalAdditionalCameraData();
        data.cameraStack.ForEach(cam => {
            if (cam != _cam) {
                cam.rect = _cam.rect;
            }
        });
    }

    public static Vector2 GetScreenSize() {
#if UNITY_EDITOR
        return UnityEditor.Handles.GetMainGameViewSize();
#else
        return new(Screen.width, Screen.height);
#endif
    }
}

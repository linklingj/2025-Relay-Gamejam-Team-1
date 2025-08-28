using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerInputs
{
    #region Defaults

    static InputActionMap actionMap;
    public static InputActionMap ActionMap
    {
        get
        {
            actionMap ??= InputSystem.actions.FindActionMap("Player", true);
            return actionMap;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Reset()
    {
        actionMap = null;
    }

    public static InputAction Get(string name)
    {
        return ActionMap.FindAction(name, true);
    }

    public static bool Enabled { get; private set; }

    public static void Enable()
    {
        ActionMap.Enable();
        Enabled = true;
    }

    public static void Disable()
    {
        ActionMap.Disable();
        Enabled = false;
    }

    #endregion

    static InputAction tapAction;
    public static InputAction TapAction
    {
        get
        {
            tapAction ??= Get("Tap");
            return tapAction;
        }
    }
    public static bool TapDown => TapAction != null && TapAction.WasPressedThisFrame();
    public static bool TapUp => TapAction != null && TapAction.WasReleasedThisFrame();
    public static bool Tapping => TapAction != null && TapAction.IsPressed();
}

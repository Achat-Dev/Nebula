using Silk.NET.Input;
using System.Collections;

namespace Nebula;

public static class Input
{
    private static readonly BitArray s_lastKeyboardState = new BitArray(349, false);
    private static readonly BitArray s_currentKeyboardState = new BitArray(349, false);

    internal static void Init(IInputContext inputContext)
    {
        Logger.EngineInfo("Initialising input");
        inputContext.Keyboards[0].KeyDown += OnKeyPressed;
        inputContext.Keyboards[0].KeyUp += OnKeyReleased;
    }

    internal static void RefreshKeyboardState()
    {
        for (int i = 0; i < s_currentKeyboardState.Count; i++)
        {
            s_lastKeyboardState[i] = s_currentKeyboardState[i];
        }
    }

    private static void OnKeyPressed(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        s_currentKeyboardState[(int)key] = true;
    }

    private static void OnKeyReleased(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        s_currentKeyboardState[(int)key] = false;
    }

    public static bool IsKeyPressed(Key key)
    {
        return !s_lastKeyboardState[(int)key] && s_currentKeyboardState[(int)key];
    }

    public static bool IsKeyDown(Key key)
    {
        return s_currentKeyboardState[(int)key];
    }

    public static bool IsKeyReleased(Key key)
    {
        return s_lastKeyboardState[(int)key] && !s_currentKeyboardState[(int)key];
    }
}

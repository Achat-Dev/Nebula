using Silk.NET.Input;

namespace Nebula;

public static class Input
{
    private static Vector2 s_mousePosition;
    private static Vector2 s_scrollDelta;
    private static InputState s_lastInputState = new InputState();
    private static InputState s_currentInputState = new InputState();

    internal static void Init(IInputContext inputContext)
    {
        Logger.EngineInfo("Initialising Input");
        inputContext.Keyboards[0].KeyDown += OnKeyPressed;
        inputContext.Keyboards[0].KeyUp += OnKeyReleased;

        inputContext.Mice[0].MouseDown += OnMouseButtonPressed;
        inputContext.Mice[0].MouseUp += OnMouseButtonReleased;
        inputContext.Mice[0].MouseMove += OnMouseMove;
        inputContext.Mice[0].Scroll += OnMouseScroll;
    }

    internal static void RefreshInputStates()
    {
        for (int i = 0; i < s_currentInputState.KeyboardState.Count; i++)
        {
            s_lastInputState.KeyboardState[i] = s_currentInputState.KeyboardState[i];
        }

        for (int i = 0; i < s_currentInputState.MouseState.Count; i++)
        {
            s_lastInputState.MouseState[i] = s_currentInputState.MouseState[i];
        }
    }

    private static void OnKeyPressed(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        s_currentInputState.KeyboardState[(int)key] = true;
    }

    private static void OnKeyReleased(IKeyboard keyboard, Silk.NET.Input.Key key, int keyCode)
    {
        s_currentInputState.KeyboardState[(int)key] = false;
    }

    private static void OnMouseButtonPressed(IMouse mouse, Silk.NET.Input.MouseButton mouseButton)
    {
        s_currentInputState.MouseState[(int)mouseButton] = true;
    }

    private static void OnMouseButtonReleased(IMouse mouse, Silk.NET.Input.MouseButton mouseButton)
    {
        s_currentInputState.MouseState[(int)mouseButton] = false;
    }

    private static void OnMouseMove(IMouse mouse, System.Numerics.Vector2 mousePosition)
    {
        s_mousePosition = mousePosition;
    }

    private static void OnMouseScroll(IMouse mouse, ScrollWheel scrollWheel)
    {
        s_scrollDelta.X = scrollWheel.X;
        s_scrollDelta.Y = scrollWheel.Y;
    }

    public static bool IsKeyPressed(Key key)
    {
        return !s_lastInputState.KeyboardState[(int)key] && s_currentInputState.KeyboardState[(int)key];
    }

    public static bool IsKeyDown(Key key)
    {
        return s_currentInputState.KeyboardState[(int)key];
    }

    public static bool IsKeyReleased(Key key)
    {
        return s_lastInputState.KeyboardState[(int)key] && !s_currentInputState.KeyboardState[(int)key];
    }

    public static bool IsMouseButtonPressed(MouseButton mouseButton)
    {
        return !s_lastInputState.MouseState[(int)mouseButton] && s_currentInputState.MouseState[(int)mouseButton];
    }

    public static bool IsMouseButtonReleased(MouseButton mouseButton)
    {
        return s_lastInputState.MouseState[(int)mouseButton] && !s_currentInputState.MouseState[(int)mouseButton];
    }

    public static Vector2 GetMousePosition()
    {
        return s_mousePosition;
    }

    public static Vector2 GetScrollDelta()
    {
        return s_scrollDelta;
    }
}

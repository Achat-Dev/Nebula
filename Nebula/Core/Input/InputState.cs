using System.Collections;

namespace Nebula;

internal struct InputState
{
    public readonly BitArray MouseState;
    public readonly BitArray KeyboardState;

    public InputState()
    {
        MouseState = new BitArray(12, false);
        KeyboardState = new BitArray(348, false);
    }
}

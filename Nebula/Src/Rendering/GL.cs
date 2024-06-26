﻿namespace Nebula.Rendering;

internal static class GL
{
    private static Silk.NET.OpenGL.GL r_gl;

    internal static void Init(Silk.NET.OpenGL.GL gl)
    {
        Logger.EngineInfo("Initialising OpenGL");
        r_gl = gl;
    }

    internal static void Dispose()
    {
        Logger.EngineInfo("Disposing OpenGL");
        r_gl.Dispose();
    }

    public static Silk.NET.OpenGL.GL Get()
    {
        return r_gl;
    }
}

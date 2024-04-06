using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public class Shader : ICacheable, IDisposable
{
    public static class Defaults
    {
        public static readonly Shader UnlitFlat = Create("Shader/Unlit_Flat.vert", "Shader/Unlit_Flat.frag", false);
        public static readonly Shader PBRFlat = Create("Shader/PBR_Flat.vert", "Shader/PBR_Flat.frag", true);
        public static readonly Shader PBRTextured = Create("Shader/PBR_Textured.vert", "Shader/PBR_Textured.frag", true);
    }

    internal enum DataType
    {
        Float,
        Vec2,
        Vec3,
        Vec4,
        Int,
        IVec2,
        IVec3,
        IVec4,
        Mat3,
        Mat4,
        Bool,
    }

    private readonly uint r_handle;
    private readonly bool r_isLit;
    private readonly Dictionary<string, int> r_uniformLocationCache = new Dictionary<string, int>();

    private Shader(string vertexPath, string geometryPath, string fragmentPath, bool isLit, ShaderCompileReplacement[] compileReplacements)
    {
        r_isLit = isLit;

        bool usesGeometryShader = !string.IsNullOrEmpty(geometryPath);

        // Create shaders
        uint vertexShaderHandle = CreateGLShader(ShaderType.VertexShader, vertexPath, compileReplacements);
        uint fragmentShaderHandle = CreateGLShader(ShaderType.FragmentShader, fragmentPath, compileReplacements);
        uint geometryShaderHandle = 0;
        if (usesGeometryShader)
        {
            geometryShaderHandle = CreateGLShader(ShaderType.GeometryShader, geometryPath, compileReplacements);
        }

        // Link shaders
        r_handle = GL.Get().CreateProgram();
        GL.Get().AttachShader(r_handle, vertexShaderHandle);
        GL.Get().AttachShader(r_handle, fragmentShaderHandle);
        if (usesGeometryShader)
        {
            GL.Get().AttachShader(r_handle, geometryShaderHandle);
        }
        GL.Get().LinkProgram(r_handle);

        // Check for errors
        GL.Get().GetProgram(r_handle, GLEnum.LinkStatus, out int status);
        if (status == 0)
        {
            Logger.EngineError("Failed to link shaders\n{0}", GL.Get().GetProgramInfoLog(r_handle));
        }

        // Clean up
        GL.Get().DetachShader(r_handle, vertexShaderHandle);
        GL.Get().DetachShader(r_handle, fragmentShaderHandle);
        GL.Get().DeleteShader(vertexShaderHandle);
        GL.Get().DeleteShader(fragmentShaderHandle);
        if (usesGeometryShader)
        {
            GL.Get().DetachShader(r_handle, geometryShaderHandle);
            GL.Get().DeleteShader(geometryShaderHandle);
        }

        // Cache uniform locations
        GL.Get().GetProgram(r_handle, GLEnum.ActiveUniforms, out int uniformCount);
        for (uint i = 0; i < uniformCount; i++)
        {
            string name = GL.Get().GetActiveUniform(r_handle, i, out _, out _);
            int location = GL.Get().GetUniformLocation(r_handle, name);
            r_uniformLocationCache.Add(name, location);
        }
    }

    public static Shader Create(string vertexPath, string fragmentPath, bool isLit, params ShaderCompileReplacement[] compileReplacements)
    {
        int hash = HashCode.Combine(vertexPath, fragmentPath);
        if (Cache.ShaderCache.TryGetValue(hash, out Shader shader))
        {
            Logger.EngineVerbose("Shader from sources {0} and {1} already exists, returning cached instance", vertexPath, fragmentPath);
            return shader;
        }

        Logger.EngineDebug("Creating shader from sources {0} and {1}", vertexPath, fragmentPath);
        shader = new Shader(vertexPath, string.Empty, fragmentPath, isLit, compileReplacements);
        Cache.ShaderCache.CacheData(hash, shader);
        return shader;
    }

    public static Shader Create(string vertexPath, string geometryPath, string fragmentPath, bool isLit, params ShaderCompileReplacement[] compileReplacements)
    {
        int hash = HashCode.Combine(vertexPath, geometryPath, fragmentPath);
        if (Cache.ShaderCache.TryGetValue(hash, out Shader shader))
        {
            Logger.EngineVerbose("Shader from sources {0}, {1} and {2} already exists, returning cached instance", vertexPath, geometryPath, fragmentPath);
            return shader;
        }

        Logger.EngineDebug("Creating shader from sources {0}, {1} and {2}", vertexPath, geometryPath, fragmentPath);
        shader = new Shader(vertexPath, geometryPath, fragmentPath, isLit, compileReplacements);
        Cache.ShaderCache.CacheData(hash, shader);
        return shader;
    }

    private uint CreateGLShader(ShaderType type, string path, ShaderCompileReplacement[] compileReplacements)
    {
        string source = ShaderParser.Parse(path, compileReplacements);

        uint handle = GL.Get().CreateShader(type);
        GL.Get().ShaderSource(handle, source);
        GL.Get().CompileShader(handle);

        string infoLog = GL.Get().GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.EngineError("Failed to compile shader {0} of type {1}\n" + infoLog, path, type);
        }

        return handle;
    }

    internal void Use()
    {
        GL.Get().UseProgram(r_handle);
    }

    internal void SetBool(int location, bool value)
    {
        GL.Get().Uniform1(location, value ? 1 : 0);
    }

    internal void SetBool(string name, bool value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform1(location, value ? 1 : 0);
        }
    }

    internal void SetFloat(int location, float value)
    {
        GL.Get().Uniform1(location, value);
    }

    internal void SetFloat(string name, float value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform1(location, value);
        }
    }

    internal void SetVec2(int location, Vector2 value)
    {
        GL.Get().Uniform2(location, value.X, value.Y);
    }

    internal void SetVec2(string name, Vector2 value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform2(location, value.X, value.Y);
        }
    }

    internal void SetVec3(int location, Vector3 value)
    {
        GL.Get().Uniform3(location, value.X, value.Y, value.Z);
    }

    internal void SetVec3(string name, Vector3 value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform3(location, value.X, value.Y, value.Z);
        }
    }

    internal void SetVec4(int location, Vector4 value)
    {
        GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    internal void SetVec4(string name, Vector4 value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
        }
    }

    internal void SetInt(int location, int value)
    {
        GL.Get().Uniform1(location, value);
    }

    internal void SetInt(string name, int value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform1(location, value);
        }
    }

    internal void SetVec2i(int location, Vector2i value)
    {
        GL.Get().Uniform2(location, value.X, value.Y);
    }

    internal void SetVec2i(string name, Vector2i value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform2(location, value.X, value.Y);
        }
    }

    internal void SetVec3i(int location, Vector3i value)
    {
        GL.Get().Uniform3(location, value.X, value.Y, value.Z);
    }

    internal void SetVec3i(string name, Vector3i value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform3(location, value.X, value.Y, value.Z);
        }
    }

    internal void SetVec4i(int location, Vector4i value)
    {
        GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    internal void SetVec4i(string name, Vector4i value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
        }
    }

    internal unsafe void SetMat3(int location, Matrix3x3 value)
    {
        GL.Get().UniformMatrix3(location, 1, false, (float*)&value);
    }

    internal unsafe void SetMat3(string name, Matrix3x3 value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().UniformMatrix3(location, 1, false, (float*)&value);
        }
    }

    internal unsafe void SetMat4(int location, Matrix4x4 value)
    {
        GL.Get().UniformMatrix4(location, 1, false, (float*)&value);
    }

    internal unsafe void SetMat4(string name, Matrix4x4 value)
    {
        if (TryGetCachedUniformLocation(name, out int location))
        {
            GL.Get().UniformMatrix4(location, 1, false, (float*)&value);
        }
    }

    internal bool TryGetCachedUniformLocation(string name, out int result)
    {
        if (r_uniformLocationCache.TryGetValue(name, out result))
        {
            return true;
        }

        result = GL.Get().GetUniformLocation(r_handle, name);

        if (result != -1)
        {
            Logger.EngineVerbose("Found uniform {0}, adding it to uniform cache", name);
            r_uniformLocationCache.Add(name, result);
            return true;
        }

        Logger.EngineError("Uniform {0} not found", name);
        return false;
    }

    internal bool IsLit()
    {
        return r_isLit;
    }

    public void Delete()
    {
        if (Cache.ShaderCache.TryGetKey(this, out int key))
        {
            Logger.EngineDebug("Deleting shader with hash {0}", key);
            Cache.ShaderCache.RemoveData(key);
        }

        IDisposable disposable = this;
        disposable.Dispose();
    }

    void IDisposable.Dispose()
    {
        r_uniformLocationCache.Clear();
        GL.Get().DeleteProgram(r_handle);
    }
}

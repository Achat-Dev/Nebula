using Silk.NET.OpenGL;
using System.Text;

namespace Nebula.Rendering;

public class Shader : ICacheable, IDisposable
{
    public static class Defaults
    {
        public static Shader UnlitFlat()
        {
            return Create("Shader/Unlit_Flat.vert", "Shader/Unlit_Flat.frag", false);
        }

        public static Shader PBRFlat()
        {
            return Create("Shader/PBR_Flat.vert", "Shader/PBR_Flat.frag", true);
        }

        public static Shader PBRTextured()
        {
            return Create("Shader/PBR_Textured.vert", "Shader/PBR_Textured.frag", true);
        }
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

    private Shader(string vertexPath, string fragmentPath, bool isLit)
    {
        r_isLit = isLit;

        // Create shaders
        uint vertexShaderHandle = CreateGLShader(ShaderType.VertexShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(vertexPath), new HashSet<string>()));
        uint fragmentShaderHandle = CreateGLShader(ShaderType.FragmentShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(fragmentPath), new HashSet<string>()));

        // Link shaders
        r_handle = GL.Get().CreateProgram();
        GL.Get().AttachShader(r_handle, vertexShaderHandle);
        GL.Get().AttachShader(r_handle, fragmentShaderHandle);
        GL.Get().LinkProgram(r_handle);

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

        // Cache uniform locations
        GL.Get().GetProgram(r_handle, GLEnum.ActiveUniforms, out int uniformCount);
        for (uint i = 0; i < uniformCount; i++)
        {
            string name = GL.Get().GetActiveUniform(r_handle, i, out _, out _);
            int location = GL.Get().GetUniformLocation(r_handle, name);
            r_uniformLocationCache.Add(name, location);
        }
    }

    private Shader(string vertexPath, string geometryPath, string fragmentPath, bool isLit)
    {
        r_isLit = isLit;

        // Create shaders
        uint vertexShaderHandle = CreateGLShader(ShaderType.VertexShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(vertexPath), new HashSet<string>()));
        uint geometryShaderHandle = CreateGLShader(ShaderType.GeometryShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(geometryPath), new HashSet<string>()));
        uint fragmentShaderHandle = CreateGLShader(ShaderType.FragmentShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(fragmentPath), new HashSet<string>()));

        // Link shaders
        r_handle = GL.Get().CreateProgram();
        GL.Get().AttachShader(r_handle, vertexShaderHandle);
        GL.Get().AttachShader(r_handle, geometryShaderHandle);
        GL.Get().AttachShader(r_handle, fragmentShaderHandle);
        GL.Get().LinkProgram(r_handle);

        GL.Get().GetProgram(r_handle, GLEnum.LinkStatus, out int status);
        if (status == 0)
        {
            Logger.EngineError("Failed to link shaders\n{0}", GL.Get().GetProgramInfoLog(r_handle));
        }

        // Clean up
        GL.Get().DetachShader(r_handle, vertexShaderHandle);
        GL.Get().DetachShader(r_handle, geometryShaderHandle);
        GL.Get().DetachShader(r_handle, fragmentShaderHandle);
        GL.Get().DeleteShader(vertexShaderHandle);
        GL.Get().DeleteShader(geometryShaderHandle);
        GL.Get().DeleteShader(fragmentShaderHandle);

        // Cache uniform locations
        GL.Get().GetProgram(r_handle, GLEnum.ActiveUniforms, out int uniformCount);
        for (uint i = 0; i < uniformCount; i++)
        {
            string name = GL.Get().GetActiveUniform(r_handle, i, out _, out _);
            int location = GL.Get().GetUniformLocation(r_handle, name);
            r_uniformLocationCache.Add(name, location);
        }
    }

    public static Shader Create(string vertexPath, string fragmentPath, bool isLit)
    {
        int hash = HashCode.Combine(vertexPath, fragmentPath);
        if (Cache.ShaderCache.TryGetValue(hash, out Shader shader))
        {
            Logger.EngineVerbose("Shader from sources {0} and {1} already exists, returning cached instance", vertexPath, fragmentPath);
            return shader;
        }

        Logger.EngineDebug("Creating shader from sources {0} and {1}", vertexPath, fragmentPath);
        shader = new Shader(vertexPath, fragmentPath, isLit);
        Cache.ShaderCache.CacheData(hash, shader);
        return shader;
    }

    public static Shader Create(string vertexPath, string geometryPath, string fragmentPath, bool isLit)
    {
        int hash = HashCode.Combine(vertexPath, geometryPath, fragmentPath);
        if (Cache.ShaderCache.TryGetValue(hash, out Shader shader))
        {
            Logger.EngineVerbose("Shader from sources {0}, {1} and {2} already exists, returning cached instance", vertexPath, geometryPath, fragmentPath);
            return shader;
        }

        Logger.EngineDebug("Creating shader from sources {0}, {1} and {2}", vertexPath, geometryPath, fragmentPath);
        shader = new Shader(vertexPath, geometryPath, fragmentPath, isLit);
        Cache.ShaderCache.CacheData(hash, shader);
        return shader;
    }

    private uint CreateGLShader(ShaderType type, string source)
    {
        uint handle = GL.Get().CreateShader(type);
        GL.Get().ShaderSource(handle, source);
        GL.Get().CompileShader(handle);

        string infoLog = GL.Get().GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.EngineError("Failed to compile shader of type {0}\n" + infoLog, type);
        }

        return handle;
    }

    private string GetSourceWithIncludes(string source, HashSet<string> includedShaders)
    {
        string include = "#include";

        if (source.Contains(include))
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] lines = source.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimStart(' ', '\t');
                if (lines[i].StartsWith(include))
                {
                    string path = lines[i].Substring(include.Length + 1).TrimEnd();
                    if (!includedShaders.Contains(path))
                    {
                        includedShaders.Add(path);
                        string includeSource = GetSourceWithIncludes(AssetLoader.LoadAsFileContent("Shader/Include/" + path), includedShaders);
                        stringBuilder.AppendLine(includeSource);
                    }
                }
                else
                {
                    stringBuilder.AppendLine(lines[i]);
                }
            }

            return stringBuilder.ToString();
        }

        return source;
    }

    internal void Use()
    {
        GL.Get().UseProgram(r_handle);
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
        GL.Get().UniformMatrix4(location, 1, false, (float*) &value);
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
            r_uniformLocationCache.Add(name, result);
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

using Silk.NET.OpenGL;
using System.Text;

namespace Nebula.Rendering;

public class Shader
{
    public enum NormalMode
    {
        UseNormals,
        NoNormals,
    }

    public enum DefaultType
    {
        Colour,
        PBRFlat,
        PBRTextured,
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

    private readonly bool r_useNormalMatrix;
    private readonly uint r_handle;
    private readonly Dictionary<string, int> r_uniformLocationCache = new Dictionary<string, int>();

    private readonly static Dictionary<(string, string), Shader> s_cache = new Dictionary<(string, string), Shader>();

    private Shader(string vertexPath, string fragmentPath, bool useNormalMatrix)
    {
        r_useNormalMatrix = useNormalMatrix;

        // Create shaders
        uint vertexShaderHandle = CreateGLShader(ShaderType.VertexShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(vertexPath)));
        uint fragmentShaderHandle = CreateGLShader(ShaderType.FragmentShader, GetSourceWithIncludes(AssetLoader.LoadAsFileContent(fragmentPath)));

        // Link shaders
        r_handle = GL.Get().CreateProgram();
        GL.Get().AttachShader(r_handle, vertexShaderHandle);
        GL.Get().AttachShader(r_handle, fragmentShaderHandle);
        GL.Get().LinkProgram(r_handle);

        GL.Get().GetProgram(r_handle, GLEnum.LinkStatus, out int status);
        if (status == 0)
        {
            Logger.EngineError($"Failed to link shaders\n{GL.Get().GetProgramInfoLog(r_handle)}");
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

    public static Shader Create(string vertexPath, string fragmentPath, bool useNormalMatrix)
    {
        if (s_cache.TryGetValue((vertexPath, fragmentPath), out Shader shader))
        {
            return shader;
        }

        Logger.EngineDebug($"Creating new shader from sources: {vertexPath} and {fragmentPath}");
        shader = new Shader(vertexPath, fragmentPath, useNormalMatrix);
        s_cache.Add((vertexPath, fragmentPath), shader);
        return shader;
    }

    public static Shader Create(DefaultType defaultType)
    {
        switch (defaultType)
        {
            case DefaultType.Colour:
                return Create("Shader/Colour.vert", "Shader/Colour.frag", false);
            case DefaultType.PBRFlat:
                return Create("Shader/PBR_Flat.vert", "Shader/PBR_Flat.frag", true);
            case DefaultType.PBRTextured:
                return Create("Shader/PBR_Textured.vert", "Shader/PBR_Textured.frag", true);
            default:
                return Create("Shader/Fallback.vert", "Shader/Fallback.frag", false);
        }
    }

    private uint CreateGLShader(ShaderType type, string source)
    {
        uint handle = GL.Get().CreateShader(type);
        GL.Get().ShaderSource(handle, source);
        GL.Get().CompileShader(handle);

        string infoLog = GL.Get().GetShaderInfoLog(handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.EngineError($"Failed to compile shader of type {type}\n{infoLog}");
        }

        return handle;
    }

    private string GetSourceWithIncludes(string source)
    {
        string include = "#include";

        if (source.Contains(include))
        {
            StringBuilder stringBuilder = new StringBuilder(source);
            string[] lines = source.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(include))
                {
                    string path = lines[i].Substring(include.Length + 1).TrimEnd();
                    string includeSource = GetSourceWithIncludes(AssetLoader.LoadAsFileContent(path));
                    stringBuilder.Replace(lines[i], includeSource);
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

    internal bool UsesNormalMatrix()
    {
        return r_useNormalMatrix;
    }

    public void Delete()
    {
        (string, string) key = ("", "");
        foreach (var item in s_cache)
        {
            if (item.Value == this)
            {
                key = item.Key;
                break;
            }
        }

        Logger.EngineDebug($"Deleting shader with sources: {key.Item1} and {key.Item2}");
        Dispose();
        s_cache.Remove(key);
    }

    internal void SetFloat(int location, float value)
    {
        GL.Get().Uniform1(location, value);
    }

    internal void SetVec2(int location, Vector2 value)
    {
        GL.Get().Uniform2(location, value.X, value.Y);
    }

    internal void SetVec3(int location, Vector3 value)
    {
        GL.Get().Uniform3(location, value.X, value.Y, value.Z);
    }

    internal void SetVec4(int location, Vector4 value)
    {
        GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    internal void SetInt(int location, int value)
    {
        GL.Get().Uniform1(location, value);
    }

    internal void SetVec2i(int location, Vector2i value)
    {
        GL.Get().Uniform2(location, value.X, value.Y);
    }

    internal void SetVec3i(int location, Vector3i value)
    {
        GL.Get().Uniform3(location, value.X, value.Y, value.Z);
    }

    internal void SetVec4i(int location, Vector4i value)
    {
        GL.Get().Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    internal unsafe void SetMat3(int location, Matrix3x3 value)
    {
        GL.Get().UniformMatrix3(location, 1, false, (float*)&value);
    }

    internal unsafe void SetMat4(int location, Matrix4x4 value)
    {
        GL.Get().UniformMatrix4(location, 1, false, (float*) &value);
    }

    internal int GetCachedUniformLocation(string name)
    {
        if (r_uniformLocationCache.TryGetValue(name, out int location))
        {
            return location;
        }

        Logger.EngineError($"Uniform \"{name}\" not found");
        return -1;
    }

    private void Dispose()
    {
        r_uniformLocationCache.Clear();
        GL.Get().DeleteProgram(r_handle);
    }

    internal static void DisposeCache()
    {
        Logger.EngineInfo("Disposing cached shaders");
        foreach (var item in s_cache)
        {
            item.Value.Dispose();
        }
        s_cache.Clear();
    }
}

namespace Nebula.Rendering;

public class ShaderInstance
{
    private readonly Shader r_shader;

    private readonly Dictionary<int, float> r_floatBuffer = new Dictionary<int, float>();
    private readonly Dictionary<int, Vector2> r_vec2Buffer = new Dictionary<int, Vector2>();
    private readonly Dictionary<int, Vector3> r_vec3Buffer = new Dictionary<int, Vector3>();
    private readonly Dictionary<int, Vector4> r_vec4Buffer = new Dictionary<int, Vector4>();
    private readonly Dictionary<int, int> r_intBuffer = new Dictionary<int, int>();
    private readonly Dictionary<int, Vector2i> r_vec2iBuffer = new Dictionary<int, Vector2i>();
    private readonly Dictionary<int, Vector3i> r_vec3iBuffer = new Dictionary<int, Vector3i>();
    private readonly Dictionary<int, Vector4i> r_vec4iBuffer = new Dictionary<int, Vector4i>();
    private readonly Dictionary<int, Matrix3x3> r_mat3Buffer = new Dictionary<int, Matrix3x3>();
    private readonly Dictionary<int, Matrix4x4> r_mat4Buffer = new Dictionary<int, Matrix4x4>();
    private readonly Dictionary<Texture, Texture.Unit> r_textureBuffer = new Dictionary<Texture, Texture.Unit>();

    public ShaderInstance(Shader shader)
    {
        r_shader = shader;
    }

    public bool GetBool(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_intBuffer[location] == 1;
        }
        return false;
    }

    public void SetBool(string name, bool value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_intBuffer.ContainsKey(location))
            {
                r_intBuffer[location] = value ? 1 : 0;
            }
            else
            {
                r_intBuffer.Add(location, value ? 1 : 0);
            }
        }
    }

    public float GetFloat(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_floatBuffer[location];
        }
        return 0f;
    }

    public void SetFloat(string name, float value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_floatBuffer.ContainsKey(location))
            {
                r_floatBuffer[location] = value;
            }
            else
            {
                r_floatBuffer.Add(location, value);
            }
        }
    }

    public Vector2 GetVec2(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec2Buffer[location];
        }
        return Vector2.Zero;
    }

    public void SetVec2(string name, Vector2 value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec2Buffer.ContainsKey(location))
            {
                r_vec2Buffer[location] = value;
            }
            else
            {
                r_vec2Buffer.Add(location, value);
            }
        }
    }

    public Vector3 GetVec3(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec3Buffer[location];
        }
        return Vector3.Zero;
    }

    public void SetVec3(string name, Vector3 value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec3Buffer.ContainsKey(location))
            {
                r_vec3Buffer[location] = value;
            }
            else
            {
                r_vec3Buffer.Add(location, value);
            }
        }
    }

    public Vector4 GetVec4(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec4Buffer[location];
        }
        return Vector4.Zero;
    }

    public void SetVec4(string name, Vector4 value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec4Buffer.ContainsKey(location))
            {
                r_vec4Buffer[location] = value;
            }
            else
            {
                r_vec4Buffer.Add(location, value);
            }
        }
    }

    public int GetInt(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_intBuffer[location];
        }
        return 0;
    }

    public void SetInt(string name, int value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_intBuffer.ContainsKey(location))
            {
                r_intBuffer[location] = value;
            }
            else
            {
                r_intBuffer.Add(location, value);
            }
        }
    }

    public Vector2i GetVec2i(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec2iBuffer[location];
        }
        return Vector2i.Zero;
    }

    public void SetVec2i(string name, Vector2i value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec2iBuffer.ContainsKey(location))
            {
                r_vec2iBuffer[location] = value;
            }
            else
            {
                r_vec2iBuffer.Add(location, value);
            }
        }
    }

    public Vector3i GetVec3i(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec3iBuffer[location];
        }
        return Vector3i.Zero;
    }

    public void SetVec3i(string name, Vector3i value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec3iBuffer.ContainsKey(location))
            {
                r_vec3iBuffer[location] = value;
            }
            else
            {
                r_vec3iBuffer.Add(location, value);
            }
        }
    }

    public Vector4i GetVec4i(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_vec4iBuffer[location];
        }
        return Vector4i.Zero;
    }

    public void SetVec4i(string name, Vector4i value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_vec4iBuffer.ContainsKey(location))
            {
                r_vec4iBuffer[location] = value;
            }
            else
            {
                r_vec4iBuffer.Add(location, value);
            }
        }
    }

    public Matrix3x3 GetMat3(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_mat3Buffer[location];
        }
        return Matrix3x3.Identity;
    }

    public void SetMat3(string name, Matrix3x3 value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_mat3Buffer.ContainsKey(location))
            {
                r_mat3Buffer[location] = value;
            }
            else
            {
                r_mat3Buffer.Add(location, value);
            }
        }
    }

    public Matrix4x4 GetMat4(string name)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            return r_mat4Buffer[location];
        }
        return Matrix4x4.Identity;
    }

    public void SetMat4(string name, Matrix4x4 value)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            if (r_mat4Buffer.ContainsKey(location))
            {
                r_mat4Buffer[location] = value;
            }
            else
            {
                r_mat4Buffer.Add(location, value);
            }
        }
    }

    public void SetTexture(string name, Texture texture, Texture.Unit textureUnit)
    {
        if (r_shader.TryGetCachedUniformLocation(name, out int location))
        {
            int textureUnitInt = (int)textureUnit - (int)Texture.Unit.Texture0;
            if (r_textureBuffer.ContainsKey(texture))
            {
                r_textureBuffer[texture] = textureUnit;
                r_intBuffer[location] = textureUnitInt;
            }
            else
            {
                r_textureBuffer.Add(texture, textureUnit);
                r_intBuffer.Add(location, textureUnitInt);
            }
        }
    }

    internal void SubmitDataToShader()
    {
        // Bind textures fist
        foreach (var item in r_textureBuffer)
        {
            item.Key.Bind(item.Value);
        }

        foreach (var item in r_floatBuffer)
        {
            r_shader.SetFloat(item.Key, item.Value);
        }
        foreach (var item in r_vec2Buffer)
        {
            r_shader.SetVec2(item.Key, item.Value);
        }
        foreach (var item in r_vec3Buffer)
        {
            r_shader.SetVec3(item.Key, item.Value);
        }
        foreach (var item in r_vec4Buffer)
        {
            r_shader.SetVec4(item.Key, item.Value);
        }
        foreach (var item in r_intBuffer)
        {
            r_shader.SetInt(item.Key, item.Value);
        }
        foreach (var item in r_vec2iBuffer)
        {
            r_shader.SetVec2i(item.Key, item.Value);
        }
        foreach (var item in r_vec3iBuffer)
        {
            r_shader.SetVec3i(item.Key, item.Value);
        }
        foreach (var item in r_vec4iBuffer)
        {
            r_shader.SetVec4i(item.Key, item.Value);
        }
        foreach (var item in r_mat3Buffer)
        {
            r_shader.SetMat3(item.Key, item.Value);
        }
        foreach (var item in r_mat4Buffer)
        {
            r_shader.SetMat4(item.Key, item.Value);
        }
    }

    public Shader GetShader()
    {
        return r_shader;
    }
}

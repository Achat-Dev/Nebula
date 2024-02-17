using Silk.NET.OpenGL;

namespace Nebula.Rendering;

public static class Renderer
{
    internal static void Init()
    {
        Logger.EngineInfo("Initialising Renderer");
        GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        GL.Get().Enable(GLEnum.DepthTest);
    }

    internal static void Clear()
    {
        GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    internal static void StartFrame(CameraComponent camera)
    {
        UniformBuffer cameraBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Camera);
        cameraBuffer.BufferData(0, camera.GetEntity().GetTransform().GetWorldPosition());

        DirectionalLight directionalLight = Lighting.GetDirectionalLight();
        Vector4 directionalLightColour = ((Vector4)directionalLight.GetColour()) * directionalLight.GetIntensity();

        UniformBuffer lightBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Lights);
        lightBuffer.BufferData(0, Lighting.GetPointLightCount());
        lightBuffer.BufferData(16, (Vector4)directionalLight.GetDirection(), directionalLightColour);
        lightBuffer.BufferData(48, Lighting.GetPointLightData());

        UniformBuffer matrixBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Matrices);
        matrixBuffer.BufferData(0, camera.GetViewProjectionMatrix());
    }

    internal static unsafe void DrawLitMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            modelMatrix.Invert();
            modelMatrix.Transpose();
            shader.SetMat3("u_modelNormalMatrix", (Matrix3x3)modelMatrix);
        }
        else
        {
            shader.SetMat3("u_modelNormalMatrix", Matrix3x3.Identity);
        }

        // Shader instance
        shader.SetVec3("u_albedo", (Vector3)shaderInstance.GetColour());
        shader.SetFloat("u_metallic", shaderInstance.GetMetallic());
        shader.SetFloat("u_roughness", shaderInstance.GetRoughness());

        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }

    internal static unsafe void DrawLitMeshTextured(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance, Texture albedoMap, Texture normalMap, Texture metallicMap, Texture roughnessMap, Texture ambientOcclusionMap)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            modelMatrix.Invert();
            modelMatrix.Transpose();
            shader.SetMat3("u_modelNormalMatrix", (Matrix3x3)modelMatrix);
        }
        else
        {
            shader.SetMat3("u_modelNormalMatrix", Matrix3x3.Identity);
        }

        // Shader instance
        albedoMap.Bind(TextureUnit.Texture0);
        normalMap.Bind(TextureUnit.Texture1);
        //metallicMap.Bind(TextureUnit.Texture2);
        roughnessMap.Bind(TextureUnit.Texture3);
        shader.SetInt("u_albedoMap", 0);
        shader.SetInt("u_normalMap", 1);
        shader.SetInt("u_metallicMap", 2);
        shader.SetInt("u_roughnessMap", 3);

        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }

    internal static unsafe void DrawUnlitMesh(VertexArrayObject vao, Shader shader, Matrix4x4 modelMatrix, Colour colour)
    {
        vao.Bind();
        shader.Use();
        shader.SetMat4("u_model", modelMatrix);
        shader.SetVec3("u_colour", (Vector3)colour);
        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }
}

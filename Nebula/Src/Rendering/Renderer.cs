using Silk.NET.OpenGL;
using System.Numerics;

namespace Nebula.Rendering;

public static class Renderer
{
    private static CameraComponent s_camera;
    private static DirectionalLight s_directionalLight;
    private static List<PointLightComponent> s_pointLights;

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
        s_camera = camera;
        s_directionalLight = Lighting.GetDirectionalLight();
        s_pointLights = Lighting.GetPointLights();

        UniformBuffer matrixBuffer = UniformBuffer.GetDefault(UniformBuffer.DefaultType.Matrices);
        matrixBuffer.BufferData(0, new Matrix4x4[] { s_camera.GetViewProjectionMatrix() });
    }

    internal static unsafe void DrawLitMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();
        shader.SetVec3("u_cameraPosition", s_camera.GetEntity().GetTransform().GetWorldPosition());

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            Matrix4x4.Invert(modelMatrix, out modelMatrix);
            modelMatrix = Matrix4x4.Transpose(modelMatrix);
            Silk.NET.Maths.Matrix3X3<float> modelNormalMatrix = new Silk.NET.Maths.Matrix3X3<float>();
            modelNormalMatrix.M11 = modelMatrix.M11;
            modelNormalMatrix.M12 = modelMatrix.M12;
            modelNormalMatrix.M13 = modelMatrix.M13;
            modelNormalMatrix.M21 = modelMatrix.M21;
            modelNormalMatrix.M22 = modelMatrix.M22;
            modelNormalMatrix.M23 = modelMatrix.M23;
            modelNormalMatrix.M31 = modelMatrix.M31;
            modelNormalMatrix.M32 = modelMatrix.M32;
            modelNormalMatrix.M33 = modelMatrix.M33;
            shader.SetMat3("u_modelNormalMatrix", modelNormalMatrix);
        }
        else
        {
            shader.SetMat3("u_modelNormalMatrix", Silk.NET.Maths.Matrix3X3<float>.Identity);
        }

        // Shader instance
        shader.SetVec3("u_albedo", (Vector3)shaderInstance.GetColour());
        shader.SetFloat("u_metallic", shaderInstance.GetMetallic());
        shader.SetFloat("u_roughness", shaderInstance.GetRoughness());

        // Directional light
        shader.SetVec3("u_directionalLight.direction", s_directionalLight.GetDirection());
        shader.SetVec3("u_directionalLight.colour", (Vector3)s_directionalLight.GetColour() * s_directionalLight.GetIntensity());

        // Point lights
        int pointLightCount = Lighting.GetPointLightCount();
        shader.SetInt("u_pointLightCount", pointLightCount);
        for (int i = 0; i < pointLightCount; i++)
        {
            shader.SetVec3($"u_pointLights[{i}].position", s_pointLights[i].GetEntity().GetTransform().GetWorldPosition());
            shader.SetVec3($"u_pointLights[{i}].colour", (Vector3)s_pointLights[i].GetColour() * s_pointLights[i].GetIntensity());
            shader.SetFloat($"u_pointLights[{i}].range", s_pointLights[i].GetRange());
        }

        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }

    internal static unsafe void DrawLitMeshTextured(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance, Texture albedoMap, Texture normalMap, Texture metallicMap, Texture roughnessMap, Texture ambientOcclusionMap)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();
        shader.SetVec3("u_cameraPosition", s_camera.GetEntity().GetTransform().GetWorldPosition());

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            Matrix4x4.Invert(modelMatrix, out modelMatrix);
            modelMatrix = Matrix4x4.Transpose(modelMatrix);
            Silk.NET.Maths.Matrix3X3<float> modelNormalMatrix = new Silk.NET.Maths.Matrix3X3<float>();
            modelNormalMatrix.M11 = modelMatrix.M11;
            modelNormalMatrix.M12 = modelMatrix.M12;
            modelNormalMatrix.M13 = modelMatrix.M13;
            modelNormalMatrix.M21 = modelMatrix.M21;
            modelNormalMatrix.M22 = modelMatrix.M22;
            modelNormalMatrix.M23 = modelMatrix.M23;
            modelNormalMatrix.M31 = modelMatrix.M31;
            modelNormalMatrix.M32 = modelMatrix.M32;
            modelNormalMatrix.M33 = modelMatrix.M33;
            shader.SetMat3("u_modelNormalMatrix", modelNormalMatrix);
        }
        else
        {
            shader.SetMat3("u_modelNormalMatrix", Silk.NET.Maths.Matrix3X3<float>.Identity);
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

        // Directional light
        shader.SetVec3("u_directionalLight.direction", s_directionalLight.GetDirection());
        shader.SetVec3("u_directionalLight.colour", (Vector3)s_directionalLight.GetColour() * s_directionalLight.GetIntensity());

        // Point lights
        int pointLightCount = Lighting.GetPointLightCount();
        shader.SetInt("u_pointLightCount", pointLightCount);
        for (int i = 0; i < pointLightCount; i++)
        {
            shader.SetVec3($"u_pointLights[{i}].position", s_pointLights[i].GetEntity().GetTransform().GetWorldPosition());
            shader.SetVec3($"u_pointLights[{i}].colour", (Vector3)s_pointLights[i].GetColour() * s_pointLights[i].GetIntensity());
            shader.SetFloat($"u_pointLights[{i}].range", s_pointLights[i].GetRange());
        }

        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }

    internal static unsafe void DrawUnlitMesh(VertexArrayObject vao, Shader shader, Matrix4x4 modelMatrix, Colour colour)
    {
        vao.Bind();
        shader.Use();
        shader.SetMat4("u_viewProjection", s_camera.GetViewProjectionMatrix());
        shader.SetMat4("u_model", modelMatrix);
        shader.SetVec3("u_colour", (Vector3)colour);
        GL.Get().DrawElements(PrimitiveType.Triangles, vao.GetIndexCount(), DrawElementsType.UnsignedInt, null);
    }
}

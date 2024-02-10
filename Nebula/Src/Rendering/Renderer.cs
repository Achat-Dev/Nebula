﻿using Silk.NET.OpenGL;
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
    }

    internal static unsafe void DrawLitMesh(VertexArrayObject vao, Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        vao.Bind();
        Shader shader = shaderInstance.GetShader();
        shader.Use();
        shader.SetMat4("u_viewProjection", s_camera.GetViewProjectionMatrix());
        shader.SetVec3("u_cameraPosition", s_camera.GetEntity().GetTransform().GetWorldPosition());

        shader.SetMat4("u_model", modelMatrix);
        if (modelMatrix.GetDeterminant() != 0f)
        {
            Matrix4x4.Invert(modelMatrix, out modelMatrix);
            modelMatrix = Matrix4x4.Transpose(modelMatrix);
            shader.SetMat4("u_modelNormalMatrix", modelMatrix);
        }
        else
        {
            shader.SetMat4("u_modelNormalMatrix", Matrix4x4.Identity);
        }

        // Shader instance
        shader.SetVec3("u_albedo", (Vector3)shaderInstance.GetColour());
        shader.SetFloat("u_metallic", shaderInstance.GetMetallic());
        shader.SetFloat("u_roughness", shaderInstance.GetRoughness());

        // Directional Light
        //shader.SetVec3("u_directionalLight.direction", s_directionalLight.GetDirection());
        //shader.SetVec3("u_directionalLight.ambient", s_directionalLight.GetAmbient());
        //shader.SetVec3("u_directionalLight.diffuse", s_directionalLight.GetDiffuse());
        //shader.SetVec3("u_directionalLight.specular", s_directionalLight.GetSpecular());

        // Point Lights
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

using Nebula.Rendering;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Nebula;

internal class Window : IDisposable
{
    private readonly IWindow m_window;

    // Temporary
    private TransformComponent m_transform = new TransformComponent();
    private TransformComponent[] m_lightTransforms = { new TransformComponent(), new TransformComponent(), new TransformComponent()};
    private Vector3[] m_lightColours = { Vector3.Right, Vector3.Up, Vector3.Forward };
    private CameraComponent m_camera;

    private BufferObject<float> m_vbo;
    private BufferObject<uint> m_ibo;
    private VertexArrayObject m_vao;
    private float[] m_vertices =
    {
        // Vertex coordinates   // Normals
         0.5f,  0.5f,  0.5f,     0.0f,  0.0f,  1.0f,  // Back face
         0.5f, -0.5f,  0.5f,     0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,     0.0f,  0.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,     0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f,  0.5f,    -1.0f,  0.0f,  0.0f,  // Left face
        -0.5f, -0.5f,  0.5f,    -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,    -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,    -1.0f,  0.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,     0.0f,  0.0f, -1.0f,  // Front face
        -0.5f, -0.5f, -0.5f,     0.0f,  0.0f, -1.0f,
         0.5f, -0.5f, -0.5f,     0.0f,  0.0f, -1.0f,
         0.5f,  0.5f, -0.5f,     0.0f,  0.0f, -1.0f,

         0.5f,  0.5f, -0.5f,     1.0f,  0.0f,  0.0f,  // Right face
         0.5f, -0.5f, -0.5f,     1.0f,  0.0f,  0.0f,
         0.5f, -0.5f,  0.5f,     1.0f,  0.0f,  0.0f,
         0.5f,  0.5f,  0.5f,     1.0f,  0.0f,  0.0f,

         0.5f,  0.5f, -0.5f,     0.0f,  1.0f,  0.0f,  // Top face
         0.5f,  0.5f,  0.5f,     0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,     0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,     0.0f,  1.0f,  0.0f,

         0.5f, -0.5f, -0.5f,     0.0f, -1.0f,  0.0f,  // Bottom face
         0.5f, -0.5f,  0.5f,     0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,     0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,     0.0f, -1.0f,  0.0f,
    };
    private uint[] m_indices =
    {
        0, 1, 2,
        0, 2, 3,
        4, 5, 6,
        4, 6, 7,
        8, 9, 10,
        8, 10, 11,
        12, 13, 14,
        12, 14, 15,
        16, 17, 18,
        16, 18, 19,
        20, 21, 22,
        20, 22, 23,
    };

    private Nebula.Rendering.Shader m_shader;
    private Nebula.Rendering.Shader m_lightSourceShader;

    public Window(string title, Vector2i size, bool vSync)
    {
        Logger.EngineInfo("Creating window");

        WindowOptions options = WindowOptions.Default;
        options.Title = title;
        options.Size = size;
        options.VSync = vSync;

        m_window = Silk.NET.Windowing.Window.Create(options);
        m_window.Load += OnLoad;
        m_window.Update += OnUpdate;
        m_window.Render += OnRender;
        m_window.Closing += OnClose;
    }

    public void Open()
    {
        Logger.EngineInfo("Opening window");
        m_window.Run();
    }

    public void Close()
    {
        Logger.EngineInfo("Closing window");
        m_window.Close();
    }

    public void Dispose()
    {
        Logger.EngineInfo("Disposing window");
        m_window.Dispose();
    }

    private void OnLoad()
    {
        Input.Init(m_window.CreateInput());
        Nebula.Rendering.GL.Init(Silk.NET.OpenGL.GL.GetApi(m_window));

        // Temporary
        m_vbo = new BufferObject<float>(m_vertices, BufferTargetARB.ArrayBuffer);
        m_ibo = new BufferObject<uint>(m_indices, BufferTargetARB.ElementArrayBuffer);
        BufferLayout bufferLayout = new BufferLayout(BufferElement.Float3, BufferElement.Float3);
        m_vao = new VertexArrayObject(m_vbo, m_ibo, bufferLayout);

        m_shader = ShaderLibrary.Get(DefaultShader.Phong);
        m_lightSourceShader = ShaderLibrary.Get(DefaultShader.Colour);

        Nebula.Rendering.GL.Get().ClearColor(System.Drawing.Color.LightBlue);
        Nebula.Rendering.GL.Get().Enable(GLEnum.DepthTest);

        Entity entity = new Entity("Camera");
        m_camera = entity.AddComponent<CameraComponent>();
        TransformComponent transform = entity.GetTransform();
        transform.Translate(new Vector3(0, 0, -5));
        for (int i = 0; i < m_lightTransforms.Length; i++)
        {
            m_lightTransforms[i].SetLocalScale(Vector3.One * 0.2f);
        }
    }

    private void OnUpdate(double deltaTime)
    {
        Scene.GetActive().Update((float)deltaTime);
        Input.RefreshInputStates();

        TransformComponent cameraTransform = m_camera.GetEntity().GetTransform();
        if (Input.IsKeyDown(Key.W)) cameraTransform.Translate(cameraTransform.GetForward() * (float)deltaTime);
        if (Input.IsKeyDown(Key.A)) cameraTransform.Translate(-cameraTransform.GetRight() * (float)deltaTime);
        if (Input.IsKeyDown(Key.S)) cameraTransform.Translate(-cameraTransform.GetForward() * (float)deltaTime);
        if (Input.IsKeyDown(Key.D)) cameraTransform.Translate(cameraTransform.GetRight() * (float)deltaTime);
        if (Input.IsKeyDown(Key.Space)) cameraTransform.Translate(cameraTransform.GetUp() * (float)deltaTime);
        if (Input.IsKeyDown(Key.ControlLeft)) cameraTransform.Translate(-cameraTransform.GetUp() * (float)deltaTime);
        if (Input.IsKeyDown(Key.Q)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.E)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Up, 40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.R)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.T)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Right, 40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.Z)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, -40 * (float)deltaTime));
        if (Input.IsKeyDown(Key.U)) cameraTransform.Rotate(Quaternion.FromAxisAngle(Vector3.Forward, 40 * (float)deltaTime));

        float piThird = (MathF.PI * 2f) / 3f;
        for (int i = 0; i < m_lightTransforms.Length; i++)
        {
            m_lightTransforms[i].SetWorldPosition(new Vector3(MathF.Sin((float)m_window.Time + piThird * i) * 2, 0, MathF.Cos((float)m_window.Time + piThird * i) * 2));
        }
    }

    private unsafe void OnRender(double deltaTime)
    {
        // Temporary
        Nebula.Rendering.GL.Get().Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        m_vao.Bind();
        m_shader.Use();

        System.Numerics.Matrix4x4 modelMatrix = m_transform.GetWorldMatrix();
        m_shader.SetMat4("u_model", modelMatrix);
        m_shader.SetMat4("u_viewProjection", m_camera.GetViewProjectionMatrix());

        if (modelMatrix.GetDeterminant() != 0f)
        {
            System.Numerics.Matrix4x4.Invert(modelMatrix, out modelMatrix);
            modelMatrix = System.Numerics.Matrix4x4.Transpose(modelMatrix);
            m_shader.SetMat4("u_modelNormalMatrix", modelMatrix);
        }
        else
        {
            m_shader.SetMat4("u_modelNormalMatrix", System.Numerics.Matrix4x4.Identity);
        }

        m_shader.SetVec3("u_cameraPosition", m_camera.GetEntity().GetTransform().GetWorldPosition());

        // Material
        Vector3 objectColour = new Vector3(1.0f, 1.0f, 1.0f);
        m_shader.SetVec3("u_material.ambient", objectColour);
        m_shader.SetVec3("u_material.diffuse", objectColour);
        m_shader.SetVec3("u_material.specular", objectColour);
        m_shader.SetFloat("u_material.shininess", 32);

        // Directional Light
        m_shader.SetVec3("u_directionalLight.direction", new Vector3(-0.2f, -1f, -0.3f));
        m_shader.SetVec3("u_directionalLight.ambient", Vector3.Zero * 0.2f);
        m_shader.SetVec3("u_directionalLight.diffuse", Vector3.Zero * 0.4f);
        m_shader.SetVec3("u_directionalLight.specular", Vector3.Zero * 0.5f);

        // Point Lights
        m_shader.SetInt("u_pointLightCount", m_lightTransforms.Length);
        for (int i = 0; i < m_lightTransforms.Length; i++)
        {
            m_shader.SetVec3($"u_pointLights[{i}].position", m_lightTransforms[i].GetWorldPosition());
            m_shader.SetVec3($"u_pointLights[{i}].ambient", m_lightColours[i] * 0.2f);
            m_shader.SetVec3($"u_pointLights[{i}].diffuse", m_lightColours[i] * 0.5f);
            m_shader.SetVec3($"u_pointLights[{i}].specular", m_lightColours[i] * 1.0f);
            m_shader.SetFloat($"u_pointLights[{i}].linearFalloff", 0.01f);
            m_shader.SetFloat($"u_pointLights[{i}].quadraticFalloff", 0.1f);
        }

        Nebula.Rendering.GL.Get().DrawElements(PrimitiveType.Triangles, (uint)m_indices.Length, DrawElementsType.UnsignedInt, null);

        // Light source
        m_lightSourceShader.Use();
        m_lightSourceShader.SetMat4("u_viewProjection", m_camera.GetViewProjectionMatrix());
        for (int i = 0; i < m_lightTransforms.Length; i++)
        {
            m_lightSourceShader.SetMat4("u_model", m_lightTransforms[i].GetWorldMatrix());
            m_lightSourceShader.SetVec3("u_colour", m_lightColours[i]);
            Nebula.Rendering.GL.Get().DrawElements(PrimitiveType.Triangles, (uint)m_indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }

    private void OnClose()
    {
        ShaderLibrary.Dispose();
        // Temporary
        m_vbo.Dispose();
        m_ibo.Dispose();
        m_vao.Dispose();
    }
}

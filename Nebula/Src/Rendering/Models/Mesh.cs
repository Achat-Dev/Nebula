using Silk.NET.OpenGL;
using AssimpFace = Silk.NET.Assimp.Face;
using AssimpMesh = Silk.NET.Assimp.Mesh;

namespace Nebula.Rendering;

internal class Mesh : IDisposable
{
    private readonly Vertex[] r_vertices;
    private readonly uint[] r_indices;
    private readonly VertexArrayObject r_vao;

    private Mesh(Vertex[] vertices, uint[] indices)
    {
        r_vertices = vertices;
        r_indices = indices;

        BufferObject<Vertex> vbo = new BufferObject<Vertex>(vertices, BufferTargetARB.ArrayBuffer);
        BufferObject<uint> ibo = new BufferObject<uint>(indices, BufferTargetARB.ElementArrayBuffer);
        r_vao = new VertexArrayObject(vbo, ibo, new BufferLayout(BufferElement.Vec3, BufferElement.Vec3, BufferElement.Vec3, BufferElement.Vec2));
    }

    public static unsafe Mesh CreateFromAssimpMesh(AssimpMesh* assimpMesh)
    {
        // Load vertices
        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < assimpMesh->MNumVertices; i++)
        {
            Vertex vertex = new Vertex();
            vertex.Position = assimpMesh->MVertices[i];

            if (assimpMesh->MNormals != null)
            {
                vertex.Normal = assimpMesh->MNormals[i];
            }

            if (assimpMesh->MTangents != null)
            {
                vertex.Tangent = assimpMesh->MTangents[i];
            }

            if (assimpMesh->MTextureCoords[0] != null)
            {
                Vector3 uv = assimpMesh->MTextureCoords[0][i];
                vertex.UV = (Vector2)uv;
            }

            vertices.Add(vertex);
        }

        // Load indices
        List<uint> indices = new List<uint>();
        for (int i = 0; i < assimpMesh->MNumFaces; i++)
        {
            AssimpFace face = assimpMesh->MFaces[i];
            for (int j = 0; j < face.MNumIndices; j++)
            {
                indices.Add(face.MIndices[j]);
            }
        }

        return new Mesh(vertices.ToArray(), indices.ToArray());
    }

    public void DrawLit(Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        Renderer.DrawLitMesh(r_vao, modelMatrix, shaderInstance);
    }

    public void Dispose()
    {
        r_vao.Dispose();
    }
}

using Silk.NET.OpenGL;
using AssimpFace = Silk.NET.Assimp.Face;
using AssimpMesh = Silk.NET.Assimp.Mesh;

namespace Nebula.Rendering;

internal class Mesh : IDisposable
{
    private float[] m_vertices;
    private uint[] m_indices;

    private readonly VertexArrayObject r_vao;

    private Mesh(float[] vertices, uint[] indices, VertexFlags vertexFlags)
    {
        m_vertices = vertices;
        m_indices = indices;

        BufferObject<float> vbo = new BufferObject<float>(vertices, BufferTargetARB.ArrayBuffer);
        BufferObject<uint> ibo = new BufferObject<uint>(indices, BufferTargetARB.ElementArrayBuffer);
        r_vao = new VertexArrayObject(vbo, ibo, vertexFlags.GenerateBufferLayout());
    }

    internal static unsafe Mesh CreateFromAssimpMesh(AssimpMesh* assimpMesh, VertexFlags vertexFlags)
    {
        // Load vertices
        int vertexCount = (int)assimpMesh->MNumVertices;
        int elementCount = vertexFlags.GetElementCount();
        float[] vertices = new float[vertexCount * elementCount];

        Vector3 position;
        Vector3 normal;
        Vector3 tangent;
        Vector3 uv;
        int count = 0;

        for (int i = 0; i < assimpMesh->MNumVertices; i++)
        {
            if (vertexFlags.HasFlag(VertexFlags.Position))
            {
                position = assimpMesh->MVertices[i];
                vertices[count++] = position.X;
                vertices[count++] = position.Y;
                vertices[count++] = position.Z;
            }

            if (vertexFlags.HasFlag(VertexFlags.Normal) && assimpMesh->MNormals != null)
            {
                normal = assimpMesh->MNormals[i];
                vertices[count++] = normal.X;
                vertices[count++] = normal.Y;
                vertices[count++] = normal.Z;
            }

            if (vertexFlags.HasFlag(VertexFlags.Tangent) && assimpMesh->MTangents != null)
            {
                tangent = assimpMesh->MTangents[i];
                vertices[count++] = tangent.X;
                vertices[count++] = tangent.Y;
                vertices[count++] = tangent.Z;
            }

            if (vertexFlags.HasFlag(VertexFlags.UV) && assimpMesh->MTextureCoords[0] != null)
            {
                uv = assimpMesh->MTextureCoords[0][i];
                vertices[count++] = uv.X;
                vertices[count++] = uv.Y;
            }
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

        return new Mesh(vertices, indices.ToArray(), vertexFlags);
    }

    internal void Draw(Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        Renderer.DrawMesh(r_vao, modelMatrix, shaderInstance);
    }

    internal VertexArrayObject GetVao()
    {
        return r_vao;
    }

    public void Dispose()
    {
        m_vertices = null;
        m_indices = null;
        r_vao.Dispose();
    }
}

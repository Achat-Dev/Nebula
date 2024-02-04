using Silk.NET.Assimp;
using System.Numerics;
using AssimpMesh = Silk.NET.Assimp.Mesh;
using AssimpNode = Silk.NET.Assimp.Node;
using AssimpScene = Silk.NET.Assimp.Scene;

namespace Nebula.Rendering;

public class Model : IDisposable
{
    private readonly List<Mesh> r_meshes = new List<Mesh>();

    private const uint c_postProcessSteps = (uint)(PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.JoinIdenticalVertices);

    private Model() { }

    public static unsafe Model Load(string path)
    {
        AssimpScene* assimpScene = Assimp.Get().ImportFileFromMemory(AssetLoader.LoadAsByteArray(path, out int dataSize), (uint)dataSize, c_postProcessSteps, "");

        if (assimpScene == null || assimpScene->MFlags == Silk.NET.Assimp.Assimp.SceneFlagsIncomplete || assimpScene->MRootNode == null)
        {
            Logger.EngineError($"Failed to load model at path \"{path}\"\n{Assimp.Get().GetErrorStringS()}");
            return null;
        }

        Model model = new Model();
        CreateFromAssimpScene(model, assimpScene, assimpScene->MRootNode);
        Assimp.Get().FreeScene(assimpScene);

        return model;
    }

    private static unsafe void CreateFromAssimpScene(Model model, AssimpScene* assimpScene, AssimpNode* assimpNode)
    {
        for (int i = 0; i < assimpNode->MNumMeshes; i++)
        {
            AssimpMesh* assimpMesh = assimpScene->MMeshes[assimpNode->MMeshes[i]];
            Mesh mesh = Mesh.CreateFromAssimpMesh(assimpMesh);
            model.AddMesh(mesh);
        }

        for (int i = 0; i < assimpNode->MNumChildren; i++)
        {
            CreateFromAssimpScene(model, assimpScene, assimpNode->MChildren[i]);
        }
    }

    private void AddMesh(Mesh mesh)
    {
        r_meshes.Add(mesh);
    }

    internal void Draw(Matrix4x4 modelMatrix, Material material)
    {
        for (int i = 0; i < r_meshes.Count; i++)
        {
            r_meshes[i].Draw(modelMatrix, material);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < r_meshes.Count; i++)
        {
            r_meshes[i].Dispose();
        }
        r_meshes.Clear();
    }
}

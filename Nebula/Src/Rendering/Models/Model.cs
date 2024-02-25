using Silk.NET.Assimp;
using AssimpMesh = Silk.NET.Assimp.Mesh;
using AssimpNode = Silk.NET.Assimp.Node;
using AssimpScene = Silk.NET.Assimp.Scene;

namespace Nebula.Rendering;

public class Model : ICacheable, IDisposable
{
    private readonly List<Mesh> r_meshes = new List<Mesh>();

    private const uint c_postProcessSteps = (uint)(PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.GenerateUVCoords | PostProcessSteps.CalculateTangentSpace | PostProcessSteps.JoinIdenticalVertices);

    private unsafe Model(AssimpScene* assimpScene, AssimpNode* assimpNode)
    {
        CreateFromAssimpScene(this, assimpScene, assimpNode);
    }

    public static unsafe Model Load(string path)
    {
        if (Cache.ModelCache.GetValue(path, out Model model))
        {
            Logger.EngineDebug($"Model at path \"{path}\" is already loaded, returning cached instance");
            return model;
        }

        Logger.EngineDebug($"Loading model at path \"{path}\"");

        AssimpScene* assimpScene = Assimp.Get().ImportFileFromMemory(AssetLoader.LoadAsByteArray(path, out int dataSize), (uint)dataSize, c_postProcessSteps, "");

        if (assimpScene == null || assimpScene->MFlags == Silk.NET.Assimp.Assimp.SceneFlagsIncomplete || assimpScene->MRootNode == null)
        {
            Logger.EngineError($"Failed to load model at path \"{path}\"\n{Assimp.Get().GetErrorStringS()}");
            return null;
        }

        model = new Model(assimpScene, assimpScene->MRootNode);
        Assimp.Get().FreeScene(assimpScene);

        Cache.ModelCache.CacheData(path, model);

        return model;
    }

    private unsafe void CreateFromAssimpScene(Model model, AssimpScene* assimpScene, AssimpNode* assimpNode)
    {
        for (int i = 0; i < assimpNode->MNumMeshes; i++)
        {
            AssimpMesh* assimpMesh = assimpScene->MMeshes[assimpNode->MMeshes[i]];
            Mesh mesh = Mesh.CreateFromAssimpMesh(assimpMesh);
            model.r_meshes.Add(mesh);
        }

        for (int i = 0; i < assimpNode->MNumChildren; i++)
        {
            CreateFromAssimpScene(model, assimpScene, assimpNode->MChildren[i]);
        }
    }

    internal void Draw(Matrix4x4 modelMatrix, ShaderInstance shaderInstance)
    {
        for (int i = 0; i < r_meshes.Count; i++)
        {
            r_meshes[i].Draw(modelMatrix, shaderInstance);
        }
    }

    public void Delete()
    {
        string key = Cache.ModelCache.GetKey(this);

        Logger.EngineDebug($"Deleting model loaded from path \"{key}\"");

        IDisposable disposable = this;
        disposable.Dispose();

        Cache.ModelCache.RemoveData(key);
    }

    void IDisposable.Dispose()
    {
        for (int i = 0; i < r_meshes.Count; i++)
        {
            r_meshes[i].Dispose();
        }
        r_meshes.Clear();
    }
}

using System.Text;

namespace Nebula.Rendering;

internal static class ShaderParser
{
    private static readonly Dictionary<string, object> s_globalCompileReplacementMap = new Dictionary<string, object>();

    internal static void Init()
    {
        Logger.EngineInfo("Initialising shader parser");

        s_globalCompileReplacementMap.Add("CASCADE_COUNT", Settings.Lighting.CascadeCount);
        s_globalCompileReplacementMap.Add("MAX_POINT_LIGHTS", Settings.Lighting.MaxPointLights);
        s_globalCompileReplacementMap.Add("MAX_DYNAMIC_SHADOW_CASTERS", Settings.Lighting.MaxDynamicShadowCasters);
    }

    public static string Parse(string source, ShaderCompileReplacement[] compileReplacements)
    {
        Dictionary<string, object> compileReplacementMap = new Dictionary<string, object>(s_globalCompileReplacementMap);
        for (int i = 0; i < compileReplacements.Length; i++)
        {
            compileReplacementMap.Add(compileReplacements[i].Name, compileReplacements[i].Value);
        }

        return ParseRecursive(source, new HashSet<string>(), compileReplacementMap);
    }

    private static string ParseRecursive(string source, HashSet<string> includedShaders, Dictionary<string, object> compileReplacementMap)
    {
        string include = "#include";
        string replace = "#replace";

        if (source.Contains(include) || source.Contains(replace))
        {
            StringBuilder stringBuilder = new StringBuilder();
            string[] lines = source.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimStart(' ', '\t');
                if (lines[i].StartsWith(include))
                {
                    string path = lines[i].Substring(include.Length + 1).TrimEnd();

                    if (!includedShaders.Contains(path))
                    {
                        includedShaders.Add(path);
                        string includeSource = ParseRecursive(AssetLoader.LoadAsFileContent("Shader/Include/" + path), includedShaders, compileReplacementMap);
                        stringBuilder.AppendLine(includeSource);
                    }
                }
                else if (lines[i].StartsWith(replace))
                {
                    string name = lines[i].Substring(replace.Length + 1).TrimEnd();

                    if (compileReplacementMap.TryGetValue(name, out object value))
                    {
                        stringBuilder.AppendLine("#define " + name + ' ' + value.ToString());
                    }
                    else
                    {
                        Logger.EngineWarn("Unknown shader compile replacement {0}", name);
                    }
                }
                else
                {
                    stringBuilder.AppendLine(lines[i]);
                }
            }

            return stringBuilder.ToString();
        }

        return source;
    }

    public static void AddGlobalCompileReplacement(string name, object value)
    {
        s_globalCompileReplacementMap.Add(name, value);
    }
}

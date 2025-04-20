using AsmResolver.DotNet;
using Il2CppInterop.Generator;
using Il2CppInterop.Generator.Runners;

namespace InteropGen;

public static class Program
{
    public static void Main(string[] args)
    {
        var gameAssemblyPath = args[0];
        var metadataPath = args[1];
        var unityVersion = args[2];
        var unityLibsDirectory = args[3];
        var outputDirectory = args[4];

        var sourceAssemblies = Cpp2IL.Wrapper.Program.Main([gameAssemblyPath, metadataPath, unityVersion]);

        var opts = new GeneratorOptions
        {
            GameAssemblyPath = gameAssemblyPath,
            Source = sourceAssemblies,
            OutputDir = outputDirectory,
            UnityBaseLibsDir = unityLibsDirectory,
            Parallel = true
        };

        Il2CppInteropGenerator.Create(opts)
            .AddInteropAssemblyGenerator()
            .Run();
    }
}
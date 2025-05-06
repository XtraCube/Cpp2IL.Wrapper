using System.Diagnostics;
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

        var dummyPath = Path.Combine(outputDirectory, "dummy");
        foreach (var assembly in sourceAssemblies)
        {
            assembly.Write(dummyPath + Path.DirectorySeparatorChar + assembly.Name);
        }

        var opts = new GeneratorOptions
        {
            GameAssemblyPath = gameAssemblyPath,
            Source = sourceAssemblies,
            OutputDir = outputDirectory,
            UnityBaseLibsDir = unityLibsDirectory,
            Parallel = true
        };

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Il2CppInteropGenerator.Create(opts)
            .AddInteropAssemblyGenerator()
            .Run();
        stopwatch.Stop();
        Console.WriteLine($"IL2CppInterop Processing took {stopwatch.ElapsedMilliseconds}ms");
    }
}
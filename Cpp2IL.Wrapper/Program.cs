using System;
using System.Collections.Generic;
using System.Diagnostics;
using AsmResolver.DotNet;
using AssetRipper.Primitives;
using Cpp2IL.Core;
using Cpp2IL.Core.Api;
using Cpp2IL.Core.InstructionSets;
using Cpp2IL.Core.OutputFormats;
using Cpp2IL.Core.ProcessingLayers;
using LibCpp2IL;

namespace Cpp2IL.Wrapper;

public static class Program
{
    public static List<AssemblyDefinition> Main(string[] args)
    {
        var gameAssemblyPath = args[0];
        var metadataPath = args[1];
        var unityVersionString = args[2];
        var unityVersion = UnityVersion.Parse(unityVersionString);

        InstructionSetRegistry.RegisterInstructionSet<NewArmV8InstructionSet>(DefaultInstructionSets.ARM_V8);
        LibCpp2IlBinaryRegistry.RegisterBuiltInBinarySupport();
        
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Cpp2IlApi.InitializeLibCpp2Il(gameAssemblyPath, metadataPath, unityVersion);

        List<Cpp2IlProcessingLayer> processingLayers = [new AttributeInjectorProcessingLayer()];

        foreach (var cpp2IlProcessingLayer in processingLayers)
        {
            cpp2IlProcessingLayer.PreProcess(Cpp2IlApi.CurrentAppContext, processingLayers);
        }

        foreach (var cpp2IlProcessingLayer in processingLayers)
        {
            cpp2IlProcessingLayer.Process(Cpp2IlApi.CurrentAppContext);
        }

        var assemblies = new AsmResolverDllOutputFormatDefault().BuildAssemblies(Cpp2IlApi.CurrentAppContext);
        
        stopwatch.Stop();
        Console.WriteLine($"Cpp2IL Processing took {stopwatch.ElapsedMilliseconds}ms");

        LibCpp2IlMain.Reset();
        Cpp2IlApi.CurrentAppContext = null;
        
        return assemblies;
    }
}
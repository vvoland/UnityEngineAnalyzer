using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace UnityEngineAnalyzer.Test
{
    static class MetadataReferenceHelper
    {
        public static readonly ImmutableList<MetadataReference> UsingUnityEngine = getRequiredMetadataReferences();

        private static ImmutableList<MetadataReference> getRequiredMetadataReferences()
        {
            var builder = ImmutableList.CreateBuilder<MetadataReference>();
            builder.Add(GetUnityMetadataReference());
            builder.AddRange(GetSystemString());

            return builder.ToImmutable();
        }

        private static MetadataReference GetUnityMetadataReference()
        {
            const string unityEngineFilePath = @"Editor\Data\Managed\UnityEngine.dll";
            var programFilesFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            var unityDirectories =
                Directory.EnumerateDirectories(programFilesFolderPath, "*Unity*", SearchOption.TopDirectoryOnly);

            foreach (var unityDirectory in unityDirectories)
            {
                var unityEngineFullPath = Path.Combine(unityDirectory, unityEngineFilePath);

                if (File.Exists(unityEngineFullPath))
                {
                    return MetadataReference.CreateFromFile(unityEngineFullPath);
                }
            }

            throw new FileNotFoundException("Unable to locate UnityEngine.dll");
        }

        private static MetadataReference[] GetSystemString()
        {
            return
                DependencyContext.Default.CompileLibraries
                .First(cl => cl.Name == "Microsoft.NETCore.App")
                .ResolveReferencePaths()
                .Select(asm => MetadataReference.CreateFromFile(asm))
                .ToArray();
        }
    }
}

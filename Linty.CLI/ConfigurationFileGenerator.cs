using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Linty.Analyzers.ForEachInUpdate;
using Microsoft.Build.Utilities;

namespace Linty.CLI
{
    /// <summary>
    /// TODO:: This Task should be run after build so the analyzerConfiguration.json gets generated with latets analyzers automatically
    /// </summary>
    public class ConfigurationFileGenerator : Task
    {
        private const string configurationFileName = "analyzerConfiguration.json";

        public override bool Execute()
        {
            var assembly = typeof(DoNotUseForEachInUpdate).Assembly;
            var allTypes = assembly.DefinedTypes;

            var rootJson = new JObject();

            foreach (var typeInfo in allTypes)
            {
                if (typeInfo.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !typeInfo.IsAbstract)
                {
                    rootJson.Add(new JProperty(typeInfo.Name, true)); //TODO SupportedDiagnostics.IsEnabledByDefault;
                }
            }

            var projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var filePath = Path.Combine(projectDir, configurationFileName);

            using (StreamWriter sw = File.CreateText(filePath))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    rootJson.WriteTo(writer);
                }
            }

            return true;
        }
    }
}

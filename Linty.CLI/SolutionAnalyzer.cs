using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Linty.Analyzers.ForEachInUpdate;
using Buildalyzer.Workspaces;
using Buildalyzer;

namespace Linty.CLI
{
    public class SolutionAnalyzer
    {
        public async Task LoadAndAnalyzeProjectAsync(FileInfo projectFile, FileInfo configFileInfo, AnalyzerReport report)
        {
            AnalyzerManager manager = new AnalyzerManager();
            ProjectAnalyzer analyzer = manager.GetProject(projectFile.FullName);

            AdhocWorkspace workspace = new AdhocWorkspace();
            Project project = analyzer.AddToWorkspace(workspace);

            var analyzerDictionary = LoadConfigFile(configFileInfo);
            var analyzers = this.GetAnalyzers(analyzerDictionary);

            await AnalyzeProject(project, analyzers, report);
        }

        private Dictionary<string, bool> LoadConfigFile(FileInfo configFile)
        {
            if (configFile == null || configFile.Exists == false)
            {
                return null;
            }

            Dictionary<string, bool> analyzerDictionary = new Dictionary<string, bool>();

            var config = File.ReadAllText(configFile.FullName); //TODO: read async?
            var jsonObject = JsonConvert.DeserializeObject<JObject>(config);

            foreach (var item in jsonObject)
            {
                analyzerDictionary.Add(item.Key, (bool)item.Value);
            }

            return analyzerDictionary;
        }

        private bool IsAnalyzerAllowedInConfiguration(Dictionary<string, bool> analyzerDictionary, string analyzerName)
        {
            if (analyzerDictionary == null || analyzerDictionary.ContainsKey(analyzerName) == false)
            {
                return true;
            }

            return analyzerDictionary[analyzerName];
        }

        private ImmutableArray<DiagnosticAnalyzer> GetAnalyzers(Dictionary<string, bool> analyzerDictionary)
        {
            var listBuilder = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            var assembly = typeof(DoNotUseForEachInUpdate).Assembly;
            var allTypes = assembly.DefinedTypes;

            foreach (var type in allTypes)
            {
                if (type.IsSubclassOf(typeof(DiagnosticAnalyzer)) && !type.IsAbstract)
                {
                    if (IsAnalyzerAllowedInConfiguration(analyzerDictionary, type.Name))
                    {
                        var instance = Activator.CreateInstance(type) as DiagnosticAnalyzer;
                        listBuilder.Add(instance);
                    }
                }
            }

            var analyzers = listBuilder.ToImmutable();
            return analyzers;
        }

        private async Task AnalyzeProject(Project project, ImmutableArray<DiagnosticAnalyzer> analyzers,
            AnalyzerReport report)
        {
            try
            {
                var compilation = await project.GetCompilationAsync();

                var diagnosticResults = await compilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync();

                report.AppendDiagnostics(diagnosticResults);
            }
            catch (Exception exception)
            {
                report.NotifyException(exception);
            }
        }
    }
}

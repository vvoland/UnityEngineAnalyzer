using CommandLine;
using System.Collections.Generic;
using UnityEngineAnalyzer;
using static Linty.CLI.Reporting.DiagnosticInfo;

namespace Linty.CLI
{
    public class Options
    {
        [Value(0, Required = true)]
        public string ProjectFile { get; set; }

        [Option('e', "exporter", HelpText = "Exporters to be used.")]
        public IEnumerable<string> Exporters { get; set; }

        [Option('c', "configuration", HelpText = "Custom json configuration to be used.")]
        public string ConfigurationFile { get; set; }

        [Option('s', "severity", Default = DiagnosticInfoSeverity.Warning, HelpText = "Minimal severity to be reported.")]
        public DiagnosticInfoSeverity MinimalSeverity { get; set; }

        [Option('v', "version", Default = UnityVersion.NONE, HelpText = "Check against spesific Unity version.")]
        public UnityVersion Version { get; set; }
    }
}
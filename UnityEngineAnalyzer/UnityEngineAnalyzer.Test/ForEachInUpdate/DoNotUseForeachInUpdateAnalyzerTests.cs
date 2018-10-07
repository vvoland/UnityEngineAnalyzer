using Linty.Analyzers;
using Linty.Analyzers.ForEachInUpdate;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.ForEachInUpdate
{
    [TestFixture]
    sealed class DoNotUseForeachInUpdateAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseForEachInUpdate();

        [Test]
        public void ForEachInUpdate()
        {
            var code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
            var colors = new[] {""red"", ""white"", ""blue""};
            var result = string.Empty;
            [|foreach|] (var color in colors)
            {
                result += color;
            }
        }
}";


             HasDiagnostic(code, DiagnosticIDs.DoNotUseForEachInUpdate);
        }
    }
}

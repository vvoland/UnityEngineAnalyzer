using Linty.Analyzers;
using Linty.Analyzers.OnGUI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.OnGUI
{
    [TestFixture]
    sealed class DoNotUseOnGUIAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseOnGUIAnalyzer();

        [Test]
        public void OnGUIUsedInMonoBehaviour()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void [|OnGUI|]() { }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseOnGUI);
        }

        [Test]
        public void OnGUIUsedInClass()
        {
            const string code = @"
class C
{
    void [|OnGUI|]() { }
}";

            NoDiagnostic(code, DiagnosticIDs.DoNotUseOnGUI);
        }
    }
}

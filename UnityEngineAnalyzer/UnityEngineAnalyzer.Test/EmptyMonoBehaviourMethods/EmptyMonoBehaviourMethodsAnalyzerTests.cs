using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;
using Linty.Analyzers;
using Linty.Analyzers.EmptyMonoBehaviourMethods;

namespace UnityEngineAnalyzer.Test.EmptyMonoBehaviourMethods
{
    [TestFixture]
    sealed class EmptyMonoBehaviourMethodsAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new EmptyMonoBehaviourMethodsAnalyzer();

        [Test]
        public void EmptyUpdateInMonoBehaviour()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    [|void Update() { }|]
}";

            HasDiagnostic(code, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }

        [Test]
        public void EmptyUpdateInNormalClass()
        {
            const string code = @"
using UnityEngine;

class C
{
    [|void Update() { }|]
}";

            NoDiagnostic(code, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }
    }
}

using Linty.Analyzers;
using Linty.Analyzers.Coroutines;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Coroutines
{
    [TestFixture]
    sealed class DoNotUseCoroutinesAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseCoroutinesAnalyzer();

        [Test]
        public void StartCoroutineUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void M()
    {
        [|StartCoroutine(""MyCoroutine"")|];
    }
}";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseCoroutines);
        }

        [Test]
        public void StartCoroutineUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void M()
    {
        [|cc.StartCoroutine(""MyCoroutine"")|];
    }
}";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseCoroutines);
        }
    }
}

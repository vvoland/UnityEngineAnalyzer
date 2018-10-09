using Linty.Analyzers;
using Linty.Analyzers.FindMethodsInUpdate;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.FindMethodsInUpdate
{
    [TestFixture]
    sealed class DoNotUseFindMethodsInUpdateAnalyzerTests : AnalyzerTestFixture
    {

        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseFindMethodsInUpdateAnalyzer();

        [Test]
        public void GameObjectFindInUpdate()
        {
            var code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        [|GameObject.Find(""param"")|];

        //var result = GameObject.Find(""param"");
    }
}";


            HasDiagnostic(code, DiagnosticIDs.DoNotUseFindMethodsInUpdate);
        }

        [Test]
        public void GameObjectFindInStart()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        [|GameObject.Find("")|];
    }
}";

            NoDiagnostic(code, DiagnosticIDs.EmptyMonoBehaviourMethod);
        }
    }
}

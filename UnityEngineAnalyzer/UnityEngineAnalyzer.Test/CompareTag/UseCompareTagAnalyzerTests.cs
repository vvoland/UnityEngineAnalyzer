using Linty.Analyzers;
using Linty.Analyzers.CompareTag;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.CompareTag
{
    [TestFixture]
    sealed class UseCompareTagAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new UseCompareTagAnalyzer();

        [Test]
        public void UseTagForComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|tag == ""Enemy""|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void UseGameObjectTagForComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|gameObject.tag == ""Enemy""|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void UseMonoBehaviourTagForComparison()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start()
    {
        var a = [|""Enemy"".Equals(cc.tag)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.UseCompareTag);
        }

        [Test]
        public void OtherEqualsComparison()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var a = [|""Player"".Equals(""Enemy"")|];
    }
}";

            NoDiagnostic(code, DiagnosticIDs.UseCompareTag);
        }
    }
}

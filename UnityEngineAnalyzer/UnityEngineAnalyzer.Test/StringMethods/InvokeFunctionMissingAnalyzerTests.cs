using Linty.Analyzers;
using Linty.Analyzers.StringMethods;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.StringMethods
{

    //TODO: this test suite needs to be more thorough - check for positive/negative cases
    // 1. the param is a constant
    // 2. the param is a variable
    // 3. the param is a literal

    [TestFixture]
    sealed class InvokeFunctionMissingAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InvokeFunctionMissingAnalyzer();

        [Test]
        public void InvokeUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { Invoke([|string.Empty|], 0f); }
}";

            HasDiagnostic(code, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { cc.Invoke([|string.Empty|], 0f); }
}";

            HasDiagnostic(code, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeRepeatingUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { InvokeRepeating([|string.Empty|], 0f, 0f); }
}";

            HasDiagnostic(code, DiagnosticIDs.InvokeFunctionMissing);
        }

        [Test]
        public void InvokeRepeatingUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { cc.InvokeRepeating([|string.Empty|], 0f, 0f); }
}";

            HasDiagnostic(code, DiagnosticIDs.InvokeFunctionMissing);
        }
    }
}

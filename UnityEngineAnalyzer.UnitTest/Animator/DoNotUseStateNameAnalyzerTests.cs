using Linty.Analyzers;
using Linty.Analyzers.Animator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Animator
{

    [TestFixture]
    sealed class DoNotSetAnimatorParameterWithNameAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStateNameAnalyzer();

        [Test]
        public void AnimatorSetFloatStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetFloat(""Run"", 1.2f)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }

        [Test]
        public void AnimatorSetIntStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetInteger(""Walk"", 1)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }

        [Test]
        public void AnimatorSetBoolStringName()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        [|animator.SetBool(""Fly"", true)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStateNameInAnimator);
        }
    }
}

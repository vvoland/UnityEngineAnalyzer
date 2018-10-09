using Linty.Analyzers;
using Linty.Analyzers.AOT;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.AOT
{
    [TestFixture]
    sealed class TypeGetTypeAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new TypeGetTypeAnalyzer();

        [Test]
        public void TypeGetTypeIsUsed()
        {
            const string code = @"
using UnityEngine;
using System;

class C : MonoBehaviour
{
    void Start()
    {
        var theType =  [|Type.GetType("""")|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.TypeGetType);
        }
    }
}

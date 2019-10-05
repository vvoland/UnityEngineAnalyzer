using Linty.Analyzers;
using Linty.Analyzers.Material;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Material
{

    [TestFixture]
    sealed class DoNotUseStringPropertyNamesAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStringPropertyNamesAnalyzer();

        [Test]
        public void MaterialGetFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.GetFloat(""_Shininess"")|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }

        [Test]
        public void MaterialSetFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.SetFloat(""_Shininess"", 1.2f)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }

        [Test]
        public void MaterialSetMatrixWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Material material;

    void Start()
    {
        [|material.SetVector(""_WaveAndDistance"", Vector3.one)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }
    }
}

using Linty.Analyzers;
using Linty.Analyzers.Shader;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Shader
{

    [TestFixture]
    sealed class DoNotUseStringPropertyNamesInShaderAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStringPropertyNamesShaderAnalyzer();

        [Test]
        public void ShaderGetGlobalFloatWithIntProperty()
        {
            const string code = @"
using UnityEngine;

class A : MonoBehaviour
{
    Shader shader;

    void Start()
    {
        [|shader.GetGlobalFloat(1)|];
    }
}";

            NoDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }

        [Test]
        public void ShaderGetGlobalFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class A : MonoBehaviour
{
    Shader shader;

    void Start()
    {
        [|shader.GetGlobalFloat(""_Shininess"")|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }

        [Test]
        public void ShaderSetGlobalFloatWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class B : MonoBehaviour
{
    Shader shader;

    void Start()
    {
        [|shader.SetGlobalFloat(""_Shininess"", 1.2f)|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }

        [Test]
        public void ShaderSetGlobalFloatArrayWithStringProperty()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    Shader shader;

    void Start()
    {
        [|shader.SetGlobalFloatArray(""_WaveAndDistance"", new float[] { 1 })|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringPropertyNames);
        }
    }
}

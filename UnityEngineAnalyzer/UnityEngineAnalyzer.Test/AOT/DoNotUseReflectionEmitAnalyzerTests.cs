using Linty.Analyzers;
using Linty.Analyzers.AOT;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;


namespace UnityEngineAnalyzer.Test.AOT
{
    [TestFixture]
    sealed class DoNotUseReflectionEmitAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseReflectionEmitAnalyzer();

        [Test]
        public void UsingReflectionEmit()
        {
            const string code = @"
[|using System.Reflection.Emit;|]

class C { }
";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseReflectionEmit);
        }

        [Test]
        public void UsingReflectionEmitNested()
        {
            const string code = @"
namespace N
{
    [|using System.Reflection.Emit;|]
    
    class C { }
}";
            HasDiagnostic(code, DiagnosticIDs.DoNotUseReflectionEmit);
        }
    }
}

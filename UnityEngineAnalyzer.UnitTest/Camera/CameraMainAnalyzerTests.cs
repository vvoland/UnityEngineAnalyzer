using Linty.Analyzers;
using Linty.Analyzers.Camera;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Camera
{
    [TestFixture]
    sealed class CameraMainAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new CameraMainAnalyzer();

        [Test]
        public void CameraMainShouldRaiseWarningOnMemberExpression()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        var orthographicSize = [|Camera.main.orthographicSize|];
    }
}";

            HasDiagnostic(code, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldRaiseWarningOnMethod()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            HasDiagnostic(code, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldThrowWarningFromCalledMethods()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Update()
    {
        Call();

        //Camera.main.transform.position = Vector3.one;
    }

    public void Call() 
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            HasDiagnostic(code, DiagnosticIDs.CameraMainIsSlow);
        }

        [Test]
        public void CameraMainShouldThrowWarningOnlyInHotPath()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start()
    {
        var size = [|Camera.main.ScreenPointToRay|](new Vector3(200, 200, 0));
    }
}";

            NoDiagnostic(code, DiagnosticIDs.CameraMainIsSlow);
        }
    }
}

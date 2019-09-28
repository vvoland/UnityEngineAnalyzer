using Linty.Analyzers;
using Linty.Analyzers.StringMethods;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.StringMethods
{
    [TestFixture]
    sealed class DoNotUseStringMethodsAnalyzerTests : AnalyzerTestFixture
    {
        protected override DiagnosticAnalyzer CreateAnalyzer() => new DoNotUseStringMethodsAnalyzer();

        [Test]
        public void SendMessageUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|SendMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|SendMessageUpwards(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedInMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    void Start() { [|BroadcastMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.SendMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.SendMessageUpwards(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedByGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    private GameObject go;
    void Start() { [|go.BroadcastMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.SendMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void SendMessageUpwardsUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.SendMessageUpwards(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }

        [Test]
        public void BroadcastMessageUsedByMonoBehaviourClass()
        {
            const string code = @"
using UnityEngine;

class CC : MonoBehaviour { }

class C : MonoBehaviour
{
    private CC cc;
    void Start() { [|cc.BroadcastMessage(string.Empty)|]; }
}";

            HasDiagnostic(code, DiagnosticIDs.DoNotUseStringMethods);
        }
    }
}

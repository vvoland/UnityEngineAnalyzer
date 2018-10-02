using Linty.Analyzers;
using Linty.Analyzers.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using RoslynNUnitLight;

namespace UnityEngineAnalyzer.Test.Transform
{
    [TestFixture]
    sealed class InstantiateAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InstantiateAnalyzer();

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarning()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        var newGameobject = [|Instantiate(prefabObject, Vector3.zero, Quaternion.identity)|];
        newGameobject.transform.SetParent(newParent.transform, false);
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarning2()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;
    Transform newTransform;

    void Update()
    {
        newTransform = [|Instantiate(prefabObject, Vector3.zero, Quaternion.identity)|] as Transform;
        newTransform.SetParent(newParent.transform, false);
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void IfParentIsSetRightAfterInstantiateRaiseWarningFindTheEvilSetParentThrougMethods()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        var newGameobject = [|Instantiate(prefabObject)|];
        EvilParentSetter(newGameobject);
    }

    void EvilParentSetter(GameObject newObj)
    { 
        newObj.transform.SetParent(newParent.transform, false);
    }
}";

            Document document;
            TextSpan span;
            TestHelpers.TryGetDocumentAndSpanFromMarkup(code, LanguageName, MetadataReferenceHelper.UsingUnityEngine, out document, out span);

            HasDiagnostic(document, span, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void SetParentShouldHappenToNewlyInstantiatedGameObject()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        [|var newGameobject = Instantiate(prefabObject);|]
        newParent.transform.SetParent(newGameobject.transform, false);
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }

        [Test]
        public void InstantiateShouldNotThrowWarningIfParentIsSetOnInstantiate()
        {
            const string code = @"
using UnityEngine;

class C : MonoBehaviour
{
    GameObject prefabObject;
    GameObject newParent;

    void Update()
    {
        [|var newGameobject = Instantiate(prefabObject, newParent.transform);|]
    }
}";

            NoDiagnostic(code, DiagnosticIDs.InstantiateShouldTakeParentArgument);
        }
    }
}

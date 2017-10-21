using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers
{
    /// <summary>
    /// This class represents a MonoBehaviour class in the Unity Project
    /// </summary>
    class MonoBehaviourInfo
    {

        private readonly INamedTypeSymbol _classSymbol;
        private readonly ClassDeclarationSyntax _classDeclaration;
        
        private static readonly ImmutableHashSet<string> UpdateMethodNames = ImmutableHashSet.Create(
            "OnGUI",
            "Update",
            "FixedUpdate",
            "LateUpdate");

        private static readonly ImmutableHashSet<string> MonoBehaviourMethods = ImmutableHashSet.Create(
            "Awake",
            "OnAnimatorIK",
            "OnAnimatorMove",
            "OnApplicationFocus",
            "OnApplicationPause",
            "OnApplicationQuit",
            "OnAudioFilterRead",
            "OnBecameInvisible",
            "OnBecameVisible",
            "OnCollisionEnter",
            "OnCollisionEnter2D",
            "OnCollisionExit",
            "OnCollisionExit2D",
            "OnCollisionStay",
            "OnCollisionStay2D",
            "OnConnectedToServer",
            "OnControllerColliderHit",
            "OnDestroy",
            "OnDisable",
            "OnDisconnectedFromServer",
            "OnDrawGizmos",
            "OnDrawGizmosSelected",
            "OnEnable",
            "OnFailedToConnect",
            "OnFailedToConnectToMasterServer",
            "OnJointBreak",
            "OnLevelWasLoaded",
            "OnMasterServerEvent",
            "OnMouseDown",
            "OnMouseDrag",
            "OnMouseEnter",
            "OnMouseExit",
            "OnMouseOver",
            "OnMouseUp",
            "OnMouseUpAsButton",
            "OnNetworkInstantiate",
            "OnParticleCollision",
            "OnPlayerConnected",
            "OnPlayerDisconnected",
            "OnPostRender",
            "OnPreCull",
            "OnPreRender",
            "OnRenderImage",
            "OnRenderObject",
            "OnSerializeNetworkView",
            "OnServerInitialized",
            "OnTransformChildrenChanged",
            "OnTransformParentChanged",
            "OnTriggerEnter",
            "OnTriggerEnter2D",
            "OnTriggerExit",
            "OnTriggerExit2D",
            "OnTriggerStay",
            "OnTriggerStay2D",
            "OnValidate",
            "OnWillRenderObject",
            "Reset",
            "Start"
            ).Union(UpdateMethodNames);

        public MonoBehaviourInfo(SyntaxNodeAnalysisContext analysisContext)
        {
            _classDeclaration = analysisContext.Node as ClassDeclarationSyntax;
            _classSymbol = analysisContext.SemanticModel.GetDeclaredSymbol(_classDeclaration) as INamedTypeSymbol;

            if (_classSymbol != null)
            {
                this.ClassName = _classSymbol.Name;
            }
        }

        public string ClassName { get; private set; }

        public void ForEachUpdateMethod(Action<MethodDeclarationSyntax> callback)
        {
            if (this.IsMonoBehaviour())
            {
                var methods = _classDeclaration.Members.OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    if (UpdateMethodNames.Contains(method.Identifier.ValueText))
                    {
                        callback(method);
                    }
                }
            }
        }

        public void ForEachMonoBehaviourMethod(Action<MethodDeclarationSyntax> callback)
        {
            if (this.IsMonoBehaviour())
            {
                var methods = _classDeclaration.Members.OfType<MethodDeclarationSyntax>();

                foreach (var method in methods)
                {
                    if (MonoBehaviourMethods.Contains(method.Identifier.ValueText))
                    {
                        callback(method);
                    }
                }
            }
        }

        public bool IsMonoBehaviour()
        {
            return IsMonoBehavior(_classSymbol);
        }

        private static bool IsMonoBehavior(INamedTypeSymbol classDeclaration)
        {
            if (classDeclaration.BaseType == null)
            {
                return false;
            }

            var baseClass = classDeclaration.BaseType;

            //TODO: Need to take into consideration if the developer extends MonoBehaviour
            if (baseClass.ContainingNamespace.Name.Equals("UnityEngine") && baseClass.Name.Equals("MonoBehaviour"))
            {
                return true;
            }

            return IsMonoBehavior(baseClass); //determine if the BaseClass extends mono behavior

        }
    }
}

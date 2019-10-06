using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.Animator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class DoNotUseStateNameAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStateName);

        private static readonly ImmutableHashSet<string> animatorStateNameMethods = ImmutableHashSet.Create(
        "GetBool",
        "GetFloat",
        "GetInteger",
        "GetVector",
        "GetQuaternion",
        "SetBool",
        "SetFloat",
        "SetInteger",
        "SetVector",
        "SetQuaternion",
        "SetTrigger",
        "PlayInFixedTime",
        "Play",
        "IsParameterControlledByCurve",
        "CrossFade",
        "CrossFadeInFixedTime");

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo();
            monoBehaviourInfo.IsStringMethod(DiagnosticDescriptors.DoNotUseStateName, "Animator", animatorStateNameMethods, context);
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Linty.Analyzers.Shader
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotUseStringPropertyNamesShaderAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStringPropertyNames);

        private static readonly ImmutableHashSet<string> shaderStringPropertyMethods = ImmutableHashSet.Create(
            "GetGlobalColor",
            "GetGlobalFloat",
            "GetGlobalFloatArray",
            "GetGlobalInt",
            "GetGlobalMatrix",
            "GetGlobalMatrixArray",
            "GetGlobalTexture",
            "GetGlobalVector",
            "GetGlobalVectorArray",
            "SetGlobalBuffer",
            "SetGlobalColor",
            "SetGlobalFloat",
            "SetGlobalFloatArray",
            "SetGlobalInt",
            "SetGlobalMatrix",
            "SetGlobalMatrixArray",
            "SetGlobalTexture",
            "SetGlobalVector",
            "SetGlobalVectorArray");

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo();
            monoBehaviourInfo.IsStringMethod(DiagnosticDescriptors.DoNotUseStringPropertyNames, "Shader", shaderStringPropertyMethods, context);
        }
    }
}

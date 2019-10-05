using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.Material
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DoNotUseStringPropertyNamesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.DoNotUseStringPropertyNames);

        private static readonly ImmutableHashSet<string> materialStringPropertyMethods = ImmutableHashSet.Create(
        "GetColor",
        "GetColorArray",
        "GetFloat",
        "GetFloatArray",
        "GetInt",
        "GetMatrix",
        "GetMatrixArray",
        "GetTexture",
        "GetTextureOffset",
        "GetTextureScale",
        "GetVector",
        "GetVectorArray",
        "SetBuffer",
        "SetColor",
        "SetColorArray",
        "SetFloat",
        "SetFloatArray",
        "SetInt",
        "SetMatrix",
        "SetMatrixArray",
        "SetTexture",
        "SetTextureOffset",
        "SetTextureScale",
        "SetVector",
        "SetVectorArray");

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var monoBehaviourInfo = new MonoBehaviourInfo();
            monoBehaviourInfo.IsStringMethod(DiagnosticDescriptors.DoNotUseStringPropertyNames, "Material", materialStringPropertyMethods, context);
        }
    }
}

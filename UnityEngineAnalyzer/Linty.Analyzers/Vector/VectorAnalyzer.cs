using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Linty.Analyzers.Vector
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class VectorAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.VectorMagnitudeIsSlow);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (context.Node as MemberAccessExpressionSyntax);

            if (memberAccess == null)
            {
                return;
            }

            SymbolInfo symbolInfo;
            if (!context.TryGetSymbolInfo(memberAccess, out symbolInfo))
            {
                return;
            }

            var containingClass = symbolInfo.Symbol?.ContainingType;

            if (containingClass != null && containingClass.ContainingNamespace.Name.Equals("UnityEngine") && (containingClass.Name.Equals("Vector3") || containingClass.Name.Equals("Vector2")))
            {
                if (symbolInfo.Symbol.Name.Equals("magnitude"))
                {
                    var diagnostic = Diagnostic.Create(SupportedDiagnostics[0], memberAccess.GetLocation(), memberAccess.Name, symbolInfo.Symbol.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
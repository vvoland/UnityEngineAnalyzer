using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.Audio
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AudioSourceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.AudioSourceMuteUsesCPU);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExpressionSyntax, SyntaxKind.ExpressionStatement);
        }

        public void AnalyzeExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var method = context.Node as ExpressionStatementSyntax;

            if (method == null)
            {
                return;
            }

            var assigment = method.Expression as AssignmentExpressionSyntax;

            if (assigment?.Left == null)
            {
                return;
            }

            SymbolInfo symbolInfo;
            if (!context.TryGetSymbolInfo(assigment.Left, out symbolInfo))
            {
                return;
            }

            if (symbolInfo.Symbol == null)
            {
                return;
            }

            var containingClass = symbolInfo.Symbol.ContainingType;
            var assigmentName = symbolInfo.Symbol.Name;

            // check if the assigment is the one from UnityEngine.AudioSource.mute
            if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("AudioSource") && assigmentName.Equals("mute"))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.AudioSourceMuteUsesCPU, method.GetLocation(), containingClass.Name, method.ToString());
                context.ReportDiagnostic(diagnostic);
            }

        }
    }
}

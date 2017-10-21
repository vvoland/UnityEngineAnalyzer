using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers
{
    abstract class LintyAnalyzer : DiagnosticAnalyzer
    {
        private Action<MonoBehaviourInfo> _forEachMonoBehaviourCallback;
        private SyntaxNodeAnalysisContext? _context;

        public sealed override void Initialize(AnalysisContext context)
        {
            if (_forEachMonoBehaviourCallback != null)
            {
                context.RegisterSyntaxNodeAction(AnalyzeMonoBehavourClassSyntax, SyntaxKind.ClassDeclaration);
            }
        }

        private void AnalyzeMonoBehavourClassSyntax(SyntaxNodeAnalysisContext context)
        {
            _context = context;

            var monoBehaviour = new MonoBehaviourInfo(context);

            if (monoBehaviour.IsMonoBehaviour())
            {
                _forEachMonoBehaviourCallback(monoBehaviour);
            }
        }

        public void ReportDiagnostic(Diagnostic diagnostic)
        {
            _context?.ReportDiagnostic(diagnostic);
        }


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        /// <summary>
        /// Allows Analyis of a Unity MonoBehaviour
        /// </summary>
        /// <param name="callback">Function to call to analyze each MonoBehaviour</param>
        public void ForEachMonoBehaviour(Action<MonoBehaviourInfo> callback)
        {
            _forEachMonoBehaviourCallback = callback;
        }
    }
}

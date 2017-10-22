using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers
{
    public abstract class LintyAnalyzer : DiagnosticAnalyzer
    {
        private Action<MonoBehaviourInfo> _forEachMonoBehaviourCallback;
        private SyntaxNodeAnalysisContext? _context;

        public abstract DiagnosticDescriptor GetDiagnosticDescriptor();

        public override void Initialize(AnalysisContext context)
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

        internal void ReportDiagnostic(Location location, string className, string methodName)
        {
            if (_context != null)
            {
                var diagnostic = Diagnostic.Create(this.GetDiagnosticDescriptor(), location, className, methodName);
                _context?.ReportDiagnostic(diagnostic);
            }
        }



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.GetDiagnosticDescriptor());

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

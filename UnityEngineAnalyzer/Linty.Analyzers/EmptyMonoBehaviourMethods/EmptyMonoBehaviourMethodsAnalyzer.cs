using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.EmptyMonoBehaviourMethods
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    sealed class EmptyMonoBehaviourMethodsAnalyzer : LintyAnalyzer
    {
        public EmptyMonoBehaviourMethodsAnalyzer()
        {
            base.ForEachMonoBehaviour(AnalyzeMonoBehaviour);
        }

        private void AnalyzeMonoBehaviour(MonoBehaviourInfo monoBehaviour)
        {
            monoBehaviour.ForEachMonoBehaviourMethod(m =>
            {
                // from the method syntax, check if there is a body and if there are statements in it
                if (m.Body?.Statements.Count == 0)
                {
                    var methodName = m.Identifier.ValueText;

                    //TODO: Move some of this logic to the LintyAnalyzer
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.EmptyMonoBehaviourMethod, m.GetLocation(), monoBehaviour.ClassName, methodName);

                    this.ReportDiagnostic(diagnostic);
                    
                }
            });
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.EmptyMonoBehaviourMethod);

    }
}

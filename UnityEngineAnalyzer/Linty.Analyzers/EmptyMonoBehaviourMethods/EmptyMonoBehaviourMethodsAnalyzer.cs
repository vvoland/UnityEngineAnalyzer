using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.EmptyMonoBehaviourMethods
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EmptyMonoBehaviourMethodsAnalyzer : LintyAnalyzer
    {
        public EmptyMonoBehaviourMethodsAnalyzer()
        {
            this.ForEachMonoBehaviour(AnalyzeMonoBehaviour);
        }

        private void AnalyzeMonoBehaviour(MonoBehaviourInfo monoBehaviour)
        {
            monoBehaviour.ForEachMonoBehaviourMethod(m =>
            {
                // from the method syntax, check if there is a body and if there are statements in it
                if (m.Body?.Statements.Count == 0)
                {
                    var methodName = m.Identifier.ValueText;

                    base.ReportDiagnostic(m.GetLocation(), monoBehaviour.ClassName, methodName);                    
                }
            });
        }

        public override DiagnosticDescriptor GetDiagnosticDescriptor()
        {
            return DiagnosticDescriptors.EmptyMonoBehaviourMethod;
        }

       // public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.EmptyMonoBehaviourMethod);

    }
}

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Linty.Analyzers.Transform
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InstantiateAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.InstantiateTakeParent);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeAssigmentExpression, SyntaxKind.SimpleAssignmentExpression); //FIND ALL SIMPLE ASSIGMENT EXPRESSIONS
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);//... OR DEFINING NEW LOCAL VARIABLE EXPRESSION
        }

        public void AnalyzeAssigmentExpression(SyntaxNodeAnalysisContext context)
        {
            var assignmentExpression = context.Node as AssignmentExpressionSyntax;
            var identifierNameSyntax = assignmentExpression.Left as IdentifierNameSyntax;

            if (identifierNameSyntax == null)
            {
                return;
            }

            var varName = identifierNameSyntax.Identifier.ToString();
            FindInstantiate(context, assignmentExpression.DescendantNodes().OfType<InvocationExpressionSyntax>(), varName);
        }

        public void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDelarationStatementSyntax = context.Node as LocalDeclarationStatementSyntax;
            var test = localDelarationStatementSyntax.Declaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();

            if (test == null || test.Identifier == null)
            {
                return;
            }

            var varName = test.Identifier.ToString();
            FindInstantiate(context, localDelarationStatementSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>(), varName);
        }

        public void FindInstantiate(SyntaxNodeAnalysisContext context, IEnumerable<InvocationExpressionSyntax> syntaxNodes, string varName)
        {
            if (string.IsNullOrEmpty(varName))
            {
                return;
            }

            foreach (var method in syntaxNodes)
            {
                SymbolInfo symbolInfo;
                if (!context.TryGetSymbolInfo(method.Expression, out symbolInfo))
                {
                    continue;
                }

                if (symbolInfo.Symbol == null)
                {
                    continue;
                }

                var containingClass = symbolInfo.Symbol.ContainingType;
                var assigmentName = symbolInfo.Symbol.Name;

                if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Transform") && symbolInfo.Symbol.Name.Equals("SetParent"))
                {
                    var dataFlow = context.SemanticModel.AnalyzeDataFlow(method);
                }

                // check if the assigment is the one from UnityEngine.Object.Instantiate
                if (containingClass.ContainingNamespace.Name.Equals("UnityEngine") && containingClass.Name.Equals("Object") && symbolInfo.Symbol.Name.Equals("Instantiate"))
                {
                    var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
                    var transformParameter = methodSymbol.Parameters.ToList().Find(
                        p => p.Type.Name.Equals("Transform")
                        && p.Type.ContainingNamespace.Name.Equals("UnityEngine"));

                    if (transformParameter != null)
                    {
                        continue;
                    }

                    //WITH THE context.Node goto parent Block. Search for all Invocations.
                      //See if there is SetParent, and it has the same var name on .LEFT
                      //And it is invoked AFTER Instantiate.


                    //TODO SEE ALSO THAT THE Variable is not assigned again in the middle....

                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.InstantiateTakeParent, method.GetLocation(), containingClass.Name, method.ToString());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}

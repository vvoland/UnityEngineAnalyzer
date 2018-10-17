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
            context.RegisterSyntaxNodeAction(AnalyzeAssigmentExpression, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
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

                    var block = context.Node.Ancestors().OfType<MethodDeclarationSyntax>().First();

                    List<int> reAssignSpan = new List<int>();
                    foreach (var assignmentExpression in block.DescendantNodes().OfType<AssignmentExpressionSyntax>())
                    {
                        var identifierNameSyntax = assignmentExpression.Left as IdentifierNameSyntax;
                        if (identifierNameSyntax?.Identifier.ToString() == varName)
                        {
                            reAssignSpan.Add(assignmentExpression.FullSpan.Start);
                        }
                    }

                    foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
                    {
                        SymbolInfo invocationSymbolnfo;
                        if (!context.TryGetSymbolInfo(invocation.Expression, out invocationSymbolnfo))
                        {
                            continue;
                        }

                        if (invocationSymbolnfo.Symbol == null)
                        {
                            continue;
                        }

                        if (invocation.FullSpan.Start < method.FullSpan.Start)
                        {
                            continue;
                        }

                        var childContainingClass = symbolInfo.Symbol.ContainingType;

                        if (childContainingClass.ContainingNamespace.Name.Equals("UnityEngine") && childContainingClass.Name.Equals("Object") && invocationSymbolnfo.Symbol.Name.Equals("SetParent"))
                        {
                            //Check that the Instantiated field is not reassigned before calling SetParent
                            bool reAssignedField = false;
                            foreach (var span in reAssignSpan)
                            {
                                if (span > method.FullSpan.Start && span < invocation.FullSpan.Start)
                                {
                                    reAssignedField = true;
                                    break;
                                }
                            }

                            if (reAssignedField)
                            {
                                continue;
                            }

                            //Check that the field that we Instantied calls SetParent & that way forces hiearchy to dirty it self
                            if (invocation.DescendantNodes().OfType<IdentifierNameSyntax>().First().Identifier.ToString() == varName)
                            {
                                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.InstantiateTakeParent, method.GetLocation(), containingClass.Name, method.ToString());
                                context.ReportDiagnostic(diagnostic);
                            }
                        }
                    }
                }
            }
        }
    }
}

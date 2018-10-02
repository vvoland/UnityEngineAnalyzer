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
            context.RegisterSyntaxNodeAction(AnalyzeClassSyntax, SyntaxKind.ClassDeclaration);
        }

        public void AnalyzeClassSyntax(SyntaxNodeAnalysisContext context)
        {
            var _classDeclaration = context.Node as ClassDeclarationSyntax;
            var methods = _classDeclaration.Members.OfType<MethodDeclarationSyntax>();

            foreach (var methodd in methods)
            {
                foreach (var method in methodd.DescendantNodes().OfType<InvocationExpressionSyntax>())
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

                        var assigment = method.Parent as AssignmentExpressionSyntax;
                        if (assigment != null)
                        {
                            var identifierName = assigment.Left as IdentifierNameSyntax;

                            SymbolInfo symbolInfo3;
                            if (!context.TryGetSymbolInfo(identifierName, out symbolInfo3))
                            {
                                continue;
                            }
                        }

                        var newVariable = method.Parent as EqualsValueClauseSyntax;
                        if (newVariable != null)
                        {
                            SymbolInfo symbolInfo2;
                            if (!context.TryGetSymbolInfo(newVariable, out symbolInfo2))
                            {
                                continue;
                            }
                        }

                        if (method.Parent is StatementSyntax || method.Parent is ExpressionSyntax)
                        {
                            var dataFlow2 = context.SemanticModel.AnalyzeDataFlow(method.Parent);
                        }

                        var dataFlow = context.SemanticModel.AnalyzeDataFlow(method);


                        //TODO GO DOWN FROM HERE.. :D


                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.InstantiateTakeParent, method.GetLocation(), containingClass.Name, method.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            //var method = context.Node as MethodDeclarationSyntax;
            //var memberAccessExpression = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>();
        }
    }
}

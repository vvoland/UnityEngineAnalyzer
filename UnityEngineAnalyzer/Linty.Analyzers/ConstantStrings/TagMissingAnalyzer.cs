using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;


namespace Linty.Analyzers.ConstantStrings
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TagMissingAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor InvokeFunctionMissing = new DiagnosticDescriptor(
              id: DiagnosticIDs.InvokeFunctionMissing,
              title: new LocalizableResourceString(nameof(InvokeFunctionMissingResources.Title), InvokeFunctionMissingResources.ResourceManager, typeof(InvokeFunctionMissingResources)),
              messageFormat: new LocalizableResourceString(nameof(InvokeFunctionMissingResources.MessageFormat), InvokeFunctionMissingResources.ResourceManager, typeof(InvokeFunctionMissingResources)),
              category: DiagnosticCategories.Performance,
              defaultSeverity: DiagnosticSeverity.Warning,
              isEnabledByDefault: true,
              description: new LocalizableResourceString(nameof(InvokeFunctionMissingResources.Description), InvokeFunctionMissingResources.ResourceManager, typeof(InvokeFunctionMissingResources))
        );

        private static readonly ImmutableHashSet<string> InvokeMethods = ImmutableHashSet.Create("Invoke", "InvokeRepeating");
        private static readonly string InvokeMethodTypeName = "UnityEngine.MonoBehaviour";


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var invocation = context.Node as InvocationExpressionSyntax;
            if (invocation == null)
            {
                return;
            }

            var methodName = invocation.MethodName();

            if (InvokeMethods.Contains(methodName))
            {
                // check if the method is the one from UnityEngine
                var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
                var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

                var fullTypeName = methodSymbol?.ContainingType.ToString();

                if (fullTypeName == InvokeMethodTypeName && invocation.ArgumentList.Arguments.Count > 0)
                {
                    var firstArgumentExpression = invocation.ArgumentList.Arguments[0];

                    var invokedMethodName = firstArgumentExpression.GetArgumentValue<string>();

                    var containingClassDeclaration = invocation.Ancestors().FirstOrDefault(a => a is ClassDeclarationSyntax) as ClassDeclarationSyntax;

                    var allMethods = containingClassDeclaration?.Members.OfType<MethodDeclarationSyntax>();

                    var invokeEndPoint = allMethods.FirstOrDefault(m => m.Identifier.ValueText == invokedMethodName);

                    if (invokeEndPoint == null)
                    {
                        var diagnostic = Diagnostic.Create(SupportedDiagnostics.First(), firstArgumentExpression.GetLocation(),
                             methodName, invokedMethodName);

                        context.ReportDiagnostic(diagnostic);
                    }
                }



            }

        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(InvokeFunctionMissingAnalyzer.InvokeFunctionMissing);
    }
}
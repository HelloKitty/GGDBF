using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GGDBF.Compiler;
using Glader.Essentials;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace GGDBF
{
	[Generator]
	public class ContextGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{

		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				ExecuteGenerator(context);
			}
			catch(System.Reflection.ReflectionTypeLoadException e)
			{
				context.AddSource("Error.txt", $"{e}\n\nLoader: {e.LoaderExceptions.Select(ex => ex.ToString()).Aggregate((s1, s2) => $"{s1}\n{s2}")}");
				throw;
			}
			catch(Exception e)
			{
				context.AddSource("Error.txt", e.ToString());

				try
				{
					context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
				}
				finally
				{
					throw e;
				}
			}
		}

		private static void ExecuteGenerator(GeneratorExecutionContext context)
		{
			INamedTypeSymbol[] symbols = context
				.GetAllTypes()
				.ToArray();

			//This finds all the context symbols trying to be generated
			var dataContextSymbols = symbols
				.Where(s => s.HasAttributeExact<RequiredDataModelAttribute>())
				.ToArray();

			foreach (var contextSymbol in dataContextSymbols)
			{
				StringBuilder builder = new StringBuilder();
				UsingsEmitter usingsEmitter = new();

				foreach (var type in contextSymbol
					.GetAttributesExact<RequiredDataModelAttribute>()
					.Select(a => (INamedTypeSymbol)a.ConstructorArguments.First().Value))
				{
					if (type.ContainingNamespace != null)
						usingsEmitter.AddNamespace(type.ContainingNamespace.FullNamespaceString());
				}

				usingsEmitter.Emit(builder);
				context.AddSource(contextSymbol.Name, builder.ToString());
			}
		}

		private static string BuildStackTrace(Exception e)
		{
			return e.StackTrace
				.Replace('{', ' ')
				.Replace('}', ' ')
				.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Skip(5)
				.Aggregate((s1, s2) => $"{s1} {s2}");
		}
	}
}

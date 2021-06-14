using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Glader.Essentials;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace GGDBF
{
	[Generator]
	public class ContextGenerator : ISourceGenerator
	{
		static ContextGenerator()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				Console.WriteLine($"Exception: {(Exception)args.ExceptionObject}");
			};
		}

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
				context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
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
				NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(CreateContextClassEmitter(contextSymbol), contextSymbol.ContainingNamespace.FullNamespaceString());

				//Default namespaces:
				usingsEmitter.AddNamespace("System");
				usingsEmitter.AddNamespace("System.Collections.Generic");
				usingsEmitter.AddNamespace("System.Threading.Tasks");
				usingsEmitter.AddNamespace("System.CodeDom.Compiler");
				usingsEmitter.AddNamespace("System.Runtime.Serialization");

				foreach(var type in RetrieveModelTypes(contextSymbol))
				{
					AddNamespacesForType(type, usingsEmitter);
				}

				usingsEmitter.Emit(builder);
				namespaceDecorator.Emit(builder);

				context.AddSource(contextSymbol.Name, ConvertFileToNode(context, builder).ToString());

				EmitSerializableModelTypes(contextSymbol, context);
			}
		}

		private static void AddNamespacesForType(INamedTypeSymbol type, UsingsEmitter usingsEmitter)
		{
			if (type.ContainingNamespace != null)
				usingsEmitter.AddNamespace(type.ContainingNamespace.FullNamespaceString());

			if (type.IsGenericType)
				foreach(var genericTypeArg in type.TypeArguments)
					if (genericTypeArg.ContainingNamespace != null)
						usingsEmitter.AddNamespace(genericTypeArg.ContainingNamespace.FullNamespaceString());
		}

		private static void EmitSerializableModelTypes(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context)
		{
			//Now we handle any potential navproperties
			foreach (var type in RetrieveModelTypes(contextSymbol))
			{
				if (!type.HasForeignKeyDefined())
					continue;

				string serializableTypeName = new ForeignKeyContainingPropertyNameParser().Parse(contextSymbol.Name, type);

				StringBuilder builder = new StringBuilder();
				UsingsEmitter usingsEmitter = new();
				NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(new SerializableTypeClassEmitter(serializableTypeName, contextSymbol.Name, type, Accessibility.Public), contextSymbol.ContainingNamespace.FullNamespaceString());

				//If the type is another namespace we should import it
				//so we don't have to use fullnames.
				AddNamespacesForType(type, usingsEmitter);

				//Default namespaces:
				usingsEmitter.AddNamespace("System");
				usingsEmitter.AddNamespace("System.Collections.Generic");
				usingsEmitter.AddNamespace("System.Threading.Tasks");
				usingsEmitter.AddNamespace("System.CodeDom.Compiler");
				usingsEmitter.AddNamespace("System.Runtime.Serialization");

				usingsEmitter.Emit(builder);
				namespaceDecorator.Emit(builder);

				context.AddSource($"{serializableTypeName}", ConvertFileToNode(context, builder).ToString());
			}
		}

		private static IEnumerable<INamedTypeSymbol> RetrieveModelTypes(INamedTypeSymbol contextSymbol)
		{
			try
			{
				return contextSymbol
					.GetAttributesExact<RequiredDataModelAttribute>()
					.Select(a => (INamedTypeSymbol)a.ConstructorArguments.First().Value);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve model types. Reason: {e}", e);
			}
		}

		private static BaseClassTypeEmitter CreateContextClassEmitter(INamedTypeSymbol contextSymbol)
		{
			var classEmitter = new ContextClassTypeEmitter(contextSymbol.Name, contextSymbol.DeclaredAccessibility);

			foreach (INamedTypeSymbol modelType in RetrieveModelTypes(contextSymbol))
			{
				classEmitter.AddProperty(RetrieveTableModelName(modelType), modelType);
			}

			return classEmitter;
		}

		private static string RetrieveTableModelName(INamedTypeSymbol modelType)
		{
			try
			{
				return new TableNameParser().Parse(modelType);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve table model name for Type: {modelType.Name}. Reason: {e}", e);
			}
		}

		private static StringBuilder ConvertFileToNode(GeneratorExecutionContext context, StringBuilder builder)
		{
			try
			{
				using AdhocWorkspace ws = new AdhocWorkspace();
				var emittableClassNode = Formatter.Format(CSharpSyntaxTree.ParseText(builder.ToString()).GetRoot(), ws);

				StringBuilder sb = new StringBuilder();

				using TextWriter classFileWriter = new StringWriter(sb);
				emittableClassNode.WriteTo(classFileWriter);

				return sb;
			}
			catch(ReflectionTypeLoadException e)
			{
				context.AddSource("Error.txt", $"{e}\n\nLoader: {e.LoaderExceptions.Select(ex => ex.ToString()).Aggregate((s1, s2) => $"{s1}\n{s2}")}");
				context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
				return builder;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to convert file to SyntaxNode. Reason: {e}");
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

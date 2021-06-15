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

			//.Where(s => !s.IsGenericType)
			foreach(var contextSymbol in dataContextSymbols)
			{
				StringBuilder builder = new StringBuilder();
				UsingsEmitter usingsEmitter = new();
				NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(CreateContextClassEmitter(contextSymbol), contextSymbol.ContainingNamespace.FullNamespaceString());

				usingsEmitter.AddNamespaces(GGDBFConstants.DEFAULT_NAMESPACES);

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
				//TODO: This is a hack because for some reason parsing the unbound generic type doesn't work
				if (type.IsUnboundGenericType)
				{
					INamedTypeSymbol typeToPass = ConvertContextOpenGenericTypeToClosedGenericType(contextSymbol, type);

					EmitSerializableTypeSource(contextSymbol, context, typeToPass);
				}
				else
					EmitSerializableTypeSource(contextSymbol, context, type);
			}
		}

		private static INamedTypeSymbol ConvertContextOpenGenericTypeToClosedGenericType(INamedTypeSymbol contextSymbol, INamedTypeSymbol type)
		{
			return contextSymbol
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.Select(p =>
				{
					if(p.Type is INamedTypeSymbol ps)
						return ps;
					return null;
				})
				.Where(t => t != null)
				.Where(t => t.IsGenericType && t.TypeArguments.Length == 2)
				.Select(t =>
				{
					if(t.TypeArguments.Last() is INamedTypeSymbol modelSymbol)
						return modelSymbol;
					else
						return null;
				})
				.Where(t => t != null)
				.First(t => t.IsGenericType && t.ConstructUnboundGenericType().Equals(type, SymbolEqualityComparer.Default));
		}

		private static void EmitSerializableTypeSource(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context, INamedTypeSymbol type)
		{
			if (!type.HasForeignKeyDefined())
				return;

			string serializableTypeName = new ForeignKeyContainingPropertyNameParser().ParseNonGeneric(contextSymbol, type);

			StringBuilder builder = new StringBuilder();
			UsingsEmitter usingsEmitter = new();
			NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(new SerializableTypeClassEmitter(serializableTypeName, type, contextSymbol, Accessibility.Public), contextSymbol.ContainingNamespace.FullNamespaceString());

			//If the type is another namespace we should import it
			//so we don't have to use fullnames.
			AddNamespacesForType(type, usingsEmitter);

			usingsEmitter.AddNamespaces(GGDBFConstants.DEFAULT_NAMESPACES);

			usingsEmitter.Emit(builder);
			namespaceDecorator.Emit(builder);

			context.AddSource($"{serializableTypeName}", ConvertFileToNode(context, builder).ToString());
		}

		private static IEnumerable<INamedTypeSymbol> RetrieveModelTypes(INamedTypeSymbol contextSymbol)
		{
			try
			{
				return contextSymbol
					.GetAttributesExact<RequiredDataModelAttribute>()
					.Select(a => (INamedTypeSymbol) a.ConstructorArguments.First().Value)
					.Where(t => t != null);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve model types. Reason: {e}", e);
			}
		}

		private static BaseClassTypeEmitter CreateContextClassEmitter(INamedTypeSymbol contextSymbol)
		{
			var classEmitter = new ContextClassTypeEmitter(contextSymbol.Name, contextSymbol, contextSymbol.DeclaredAccessibility);

			foreach (INamedTypeSymbol modelType in RetrieveModelTypes(contextSymbol))
			{
				if (modelType.IsUnboundGenericType)
				{
					INamedTypeSymbol typeToPass = ConvertContextOpenGenericTypeToClosedGenericType(contextSymbol, modelType);

					classEmitter.AddProperty(RetrieveTableModelName(modelType), typeToPass, true);
				}
				else
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

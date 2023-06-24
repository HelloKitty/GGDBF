using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
			AppDomain.CurrentDomain.AssemblyResolve += ResolveAnnotationsAssembly;
		}

		private HashSet<string> EmittedKeyTypes { get; } = new HashSet<string>();

		//Runtime binding redirection maybe?? This may have helped fix a case where the annotations library wasn't loaded.
		private static Assembly ResolveAnnotationsAssembly(object sender, ResolveEventArgs args)
		{
			AssemblyName requestedAssembly = new AssemblyName(args.Name);
			if(requestedAssembly.Name == "System.ComponentModel.Annotations")
			{
				//Prevents failed load stackoverflows
				AppDomain.CurrentDomain.AssemblyResolve -= ResolveAnnotationsAssembly;
				return Assembly.Load(requestedAssembly.Name);
			}

			return null;
		}

		public void Initialize(GeneratorInitializationContext context)
		{
#if DEBUG && false
			// Attach the debugger.
			if(!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
			else
				throw new InvalidOperationException($"Failed to hook debugger.");
#endif
		}

		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				ExecuteGenerator(context, context.CancellationToken);
			}
			catch(System.Reflection.ReflectionTypeLoadException e)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return;

				context.AddSource("Error.txt", $"{e}\n\nLoader: {e.LoaderExceptions.Select(ex => ex.ToString()).Aggregate((s1, s2) => $"{s1}\n{s2}")}");
				context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("FCC001", "Compiler Failure", $"Error: {e.GetType().Name}. Failed: {e.Message} Stack: {{0}}", "Error", DiagnosticSeverity.Error, true), Location.None, BuildStackTrace(e)));
				throw;
			}
			catch(Exception e)
			{
				if (context.CancellationToken.IsCancellationRequested)
					return;

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

		private void ExecuteGenerator(GeneratorExecutionContext context, CancellationToken token)
		{
			INamedTypeSymbol[] symbols = context
				.GetAllTypes()
				.ToArray();

			if (token.IsCancellationRequested)
				return;

			//This finds all the context symbols trying to be generated
			var dataContextSymbols = symbols
				.Where(s => s.HasAttributeExact<RequiredDataModelAttribute>())
				.ToArray();

			//.Where(s => !s.IsGenericType)
			foreach(var contextSymbol in dataContextSymbols)
			{
				if (token.IsCancellationRequested)
					return;

				StringBuilder builder = new StringBuilder();
				UsingsEmitter usingsEmitter = new();

				// Can be null if token cancelled
				var emitter = CreateContextClassEmitter(contextSymbol, token);
				if (token.IsCancellationRequested)
					return;

				NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(emitter, contextSymbol.ContainingNamespace.FullNamespaceString());

				usingsEmitter.AddNamespaces(GGDBFConstants.DEFAULT_NAMESPACES);

				foreach(var type in RetrieveModelTypes(contextSymbol))
				{
					if (token.IsCancellationRequested)
						return;

					AddNamespacesForType(type, usingsEmitter, token);
				}

				if (token.IsCancellationRequested)
					return;

				usingsEmitter.Emit(builder, token);
				namespaceDecorator.Emit(builder, token);

				if (token.IsCancellationRequested)
					return;

				context.AddSource(contextSymbol.Name, ConvertFileToNode(context, builder).ToString());

				EmitSerializableModelTypes(contextSymbol, context, token);
				EmitModelKeyTypes(contextSymbol, context, token);
			}
		}

		private static void AddNamespacesForType(INamedTypeSymbol type, UsingsEmitter usingsEmitter, CancellationToken token)
		{
			if (type.ContainingNamespace != null)
				usingsEmitter.AddNamespace(type.ContainingNamespace.FullNamespaceString());

			if (type.IsGenericType)
				foreach (var genericTypeArg in type.TypeArguments)
				{
					if (token.IsCancellationRequested)
						return;

					if(genericTypeArg.ContainingNamespace != null)
						usingsEmitter.AddNamespace(genericTypeArg.ContainingNamespace.FullNamespaceString());
				}
		}

		private void EmitModelKeyTypes(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context, 
			CancellationToken token)
		{
			foreach(var type in RetrieveModelTypes(contextSymbol))
			{
				if (token.IsCancellationRequested)
					return;

				//TODO: This is a hack because for some reason parsing the unbound generic type doesn't work
				if(type.IsUnboundGenericType)
				{
					INamedTypeSymbol typeToPass = ConvertContextOpenGenericTypeToClosedGenericType(contextSymbol, type);

					if (!typeToPass.HasAttributeExact<CompositeKeyHintAttribute>())
						continue;

					EmitModelKeyTypeSource(contextSymbol, context, typeToPass, token);
				}
				else
				{
					if(!type.HasAttributeExact<CompositeKeyHintAttribute>())
						continue;

					EmitModelKeyTypeSource(contextSymbol, context, type, token);
				}
			}
		}

		private static void EmitSerializableModelTypes(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context, 
			CancellationToken token)
		{
			//Now we handle any potential navproperties
			foreach (var type in RetrieveModelTypes(contextSymbol))
			{
				if (token.IsCancellationRequested)
					return;

				//TODO: This is a hack because for some reason parsing the unbound generic type doesn't work
				if (type.IsUnboundGenericType)
				{
					INamedTypeSymbol typeToPass = ConvertContextOpenGenericTypeToClosedGenericType(contextSymbol, type);

					EmitSerializableTypeSource(contextSymbol, context, typeToPass, token);
				}
				else
					EmitSerializableTypeSource(contextSymbol, context, type, token);
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

		private void EmitModelKeyTypeSource(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context, INamedTypeSymbol type, 
			CancellationToken token)
		{
			string keyName = new TablePrimaryKeyParser().ParseSimple(type);

			StringBuilder builder = new StringBuilder();
			UsingsEmitter usingsEmitter = new();
			NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(new CompositeKeyTypeEmitter(keyName, type, Accessibility.Public), contextSymbol.ContainingNamespace.FullNamespaceString());

			//If the type is another namespace we should import it
			//so we don't have to use fullnames.
			AddNamespacesForType(type, usingsEmitter, token);

			usingsEmitter.AddNamespaces(GGDBFConstants.DEFAULT_NAMESPACES);

			usingsEmitter.Emit(builder, token);
			namespaceDecorator.Emit(builder, token);

			string source = builder.ToString();
			string hashMapKey = $"{keyName} {source.Substring(source.IndexOf('{'))}";
			//Keys could be shared in cases of multiple contexts
			if(EmittedKeyTypes.Contains(hashMapKey))
				return;

			EmittedKeyTypes.Add(hashMapKey);
			context.AddSource($"{contextSymbol.Name}_{keyName}", ConvertFileToNode(context, builder).ToString());
		}

		private static void EmitSerializableTypeSource(INamedTypeSymbol contextSymbol, GeneratorExecutionContext context, INamedTypeSymbol type, 
			CancellationToken token)
		{
			if (!type.HasForeignKeyDefined() && !type.HasOwnedTypePropertyWithForeignKey())
				return;

			string serializableTypeName = new ForeignKeyContainingPropertyNameParser().ParseNonGeneric(contextSymbol, type);

			StringBuilder builder = new StringBuilder();
			UsingsEmitter usingsEmitter = new();
			NamespaceDecoratorEmitter namespaceDecorator = new NamespaceDecoratorEmitter(new SerializableTypeClassEmitter(serializableTypeName, type, contextSymbol, Accessibility.Public), contextSymbol.ContainingNamespace.FullNamespaceString());

			//If the type is another namespace we should import it
			//so we don't have to use fullnames.
			AddNamespacesForType(type, usingsEmitter, token);

			//Add namespaces for each property
			foreach (var t in type
				.GetMembers()
				.Where(m => m.Kind == SymbolKind.Property)
				.Cast<IPropertySymbol>()
				.Select(p => p.Type)
				.Select(t => t as INamedTypeSymbol)
				.Where(t => t != null))
			{
				if (token.IsCancellationRequested)
					return;

				AddNamespacesForType(t, usingsEmitter, token);
			}

			usingsEmitter.AddNamespaces(GGDBFConstants.DEFAULT_NAMESPACES);

			usingsEmitter.Emit(builder, token);
			namespaceDecorator.Emit(builder, token);

			if (token.IsCancellationRequested)
				return;

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

		/// <summary>
		/// Returns null if token is cancelled.
		/// </summary>
		/// <param name="contextSymbol"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private static BaseClassTypeEmitter CreateContextClassEmitter(INamedTypeSymbol contextSymbol, CancellationToken token)
		{
			var classEmitter = new ContextClassTypeEmitter(contextSymbol.Name, contextSymbol, contextSymbol.DeclaredAccessibility);

			foreach (INamedTypeSymbol modelType in RetrieveModelTypes(contextSymbol))
			{
				if (token.IsCancellationRequested)
					return null;

				//TODO: Class emitter works for unbound generic types EXCEPT for determining if it is a foreign key type.
				if (modelType.IsUnboundGenericType)
				{
					INamedTypeSymbol typeToPass = ConvertContextOpenGenericTypeToClosedGenericType(contextSymbol, modelType);

					classEmitter.AddProperty(RetrieveTableModelName(modelType), typeToPass, true, modelType.HasAttributeExact<MutableModelTableAttribute>(), modelType.HasAttributeExact<RuntimeModelTableAttribute>());
				}
				else
					classEmitter.AddProperty(RetrieveTableModelName(modelType), modelType, false, modelType.HasAttributeExact<MutableModelTableAttribute>(), modelType.HasAttributeExact<RuntimeModelTableAttribute>());
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

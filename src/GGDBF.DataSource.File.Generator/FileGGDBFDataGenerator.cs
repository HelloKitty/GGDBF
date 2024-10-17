using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GGDBF
{
	public sealed class FileGGDBFDataGenerator<TGGDBFContextType>
		where TGGDBFContextType : class, IGGDBFContext
	{
		/// <summary>
		/// The base URL for the requests.
		/// </summary>
		private string BasePath { get; }

		private IGGDBFSerializer SerializationStrategy { get; }

		public FileGGDBFDataGenerator(string basPath, 
			IGGDBFSerializer serializationStrategy)
		{
			if (string.IsNullOrWhiteSpace(basPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(basPath));
			BasePath = basPath;
			SerializationStrategy = serializationStrategy ?? throw new ArgumentNullException(nameof(serializationStrategy));
		}

		public async Task GenerateFilesAsync(TGGDBFContextType context, CancellationToken token = default)
		{
			// Walk and find all IGGDBFWriteable props and write
			foreach(var prop in typeof(TGGDBFContextType).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				try
				{
					await GenerateFileAsync(context, token, prop);
				}
				catch(Exception e)
				{
					throw new InvalidOperationException($"Failed to generate GGDBF file for Prop: {prop.Name} Type: {prop.PropertyType.Name}. Reason: {e}", e);
				}
			}
		}

		private async Task GenerateFileAsync(TGGDBFContextType context, CancellationToken token, PropertyInfo prop)
		{
			// Now, we must check each actual prop value if it matches writable table
			if (prop.GetMethod == null)
				return;

			var candidate = prop.GetValue(context);

			if (candidate == null)
				return;

			if (candidate is IMutableGGDBFTable)
				return;

			var table = CreateGGDBFTable(candidate);

			if (table == null)
				throw new InvalidOperationException($"Failed to generate Table for Prop: {prop.PropertyType.Name}");

			await (table as IGGDBFWriteable).WriteAsync(new FileGGDBFDataWriter(SerializationStrategy, BasePath), token);
		}

		object CreateGGDBFTable(object candidate)
		{
			if (candidate == null) throw new ArgumentNullException(nameof(candidate));

			IEnumerable values = RetrieveTableModelTypeFromCanidateProp(candidate);

			// We need to get the true type
			Type modelType = RetrieveModelType(values, candidate);

			if(modelType != candidate.GetType().GenericTypeArguments.Last())
			{
				var tableType = typeof(GGDBFTable<,>).MakeGenericType(candidate.GetType().GenericTypeArguments.First(), candidate.GetType().GenericTypeArguments.Last());

				var table = Activator.CreateInstance(tableType);

				if (table == null)
					throw new InvalidOperationException($"Failed to create Table Type: {tableType}");

				table.GetType().GetProperty(nameof(GGDBFTable<int, int>.TableName)).SetValue(table, GetGGDBFTableName(candidate));
				table.GetType().GetProperty(nameof(GGDBFTable<int, int>.TableData)).SetValue(table, candidate);

				try
				{
					// Convert Canidate collection to serializable table
					// GGDBFTable<TPrimaryKeyType, TSerializableModelType> ConvertTo<TPrimaryKeyType, TModelType, TSerializableModelType>
					return typeof(GGDBFTableExtensions).GetMethod(nameof(GGDBFTableExtensions.ConvertTo), BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod).MakeGenericMethod(new Type[]
					{
						candidate.GetType().GenericTypeArguments.First(),
						candidate.GetType().GenericTypeArguments.Last(),
						modelType
					}).Invoke(null, new[] { table });
				}
				catch (Exception e)
				{
					throw new InvalidOperationException($"Failed to call {nameof(GGDBFTableExtensions.ConvertTo)}", e);
				}
			}
			else
			{
				var tableType = typeof(GGDBFTable<,>).MakeGenericType(candidate.GetType().GenericTypeArguments.First(), modelType);

				var table = Activator.CreateInstance(tableType);

				table.GetType().GetProperty(nameof(GGDBFTable<int, int>.TableName)).SetValue(table, GetGGDBFTableName(candidate));
				table.GetType().GetProperty(nameof(GGDBFTable<int, int>.TableData)).SetValue(table, candidate);

				return table;
			}
		}

		private static IEnumerable RetrieveTableModelTypeFromCanidateProp(object candidate)
		{
			try
			{
				return (IEnumerable)candidate.GetType().GetProperty(nameof(Dictionary<int, int>.Values)).GetValue(candidate);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Failed to retrieve Values {nameof(IEnumerable)} from Prop: {candidate.GetType()}", e);
			}
		}

		private object GetGGDBFTableName(object candidate)
		{
			var name = candidate.GetType().GenericTypeArguments.Last().GetCustomAttribute<TableAttribute>(true).Name;

			return name.Replace("_", "");
		}

		private static Type RetrieveModelType(IEnumerable values, object candidate)
		{
			var enumerator = values.GetEnumerator();
			if (enumerator.MoveNext())
			{
				return enumerator.Current.GetType();
			}
			else
			{
				return candidate.GetType().GenericTypeArguments.Last();
			}
		}
	}
}

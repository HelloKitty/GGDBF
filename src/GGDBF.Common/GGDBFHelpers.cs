using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GGDBF
{
	public static class GGDBFHelpers
	{
		public static SerializableGGDBFCollection<TKeyType, TModelType> CreateSerializableCollection<TKeyType, TModelType>(Func<TModelType, TKeyType> keyResolutionFunction, IEnumerable<TModelType> models)
		{
			if (keyResolutionFunction == null) throw new ArgumentNullException(nameof(keyResolutionFunction));
			if (models == null) throw new ArgumentNullException(nameof(models));

			//Single enumeration
			TModelType[] modelsArray = models as TModelType[] ?? models.ToArray();
			TKeyType[] keys = new TKeyType[modelsArray.Length];
			
			//Initialize the keys in an efficient manner
			for (int i = 0; i < modelsArray.Length; i++)
				keys[i] = keyResolutionFunction(modelsArray[i]);

			return new SerializableGGDBFCollection<TKeyType, TModelType>(keys);
		}

		/// <summary>
		/// Calls the static Initialize function for the specified GGDBF Context type using the provided
		/// data source.
		/// </summary>
		/// <typeparam name="TGGDBFContextType">Context type.</typeparam>
		/// <param name="source">The data source.</param>
		/// <returns>Awaitable for when the Initialize method finishes.</returns>
		public static async Task CallInitialize<TGGDBFContextType>(IGGDBFDataSource source)
			where TGGDBFContextType : IGGDBFContext
		{
			await (Task)typeof(TGGDBFContextType)
				.GetMethod(GGDBFConstants.INITIALIZE_METHOD_NAME, BindingFlags.Static | BindingFlags.Public)
				.Invoke(null, new object[1] { source });
		}

		/// <summary>
		/// Retrieves the specified GGDBF Context type's singleton.
		/// </summary>
		/// <typeparam name="TGGDBFContextType">Context type.</typeparam>
		/// <returns>The context instance.</returns>
		public static TGGDBFContextType GetInstance<TGGDBFContextType>()
			where TGGDBFContextType : IGGDBFContext
		{
			//TODO: This is slow
			return (TGGDBFContextType) typeof(TGGDBFContextType)
				.GetProperty(GGDBFConstants.CONTEXT_SINGLETON_PROPERTY_NAME, BindingFlags.Public | BindingFlags.Static)
				.GetValue(null);
		}

		/// <summary>
		/// Indicates if the GGDBF Context singleton is initialized.
		/// </summary>
		/// <typeparam name="TGGDBFContextType">Context type.</typeparam>
		/// <returns>True if the context is initialized.</returns>
		public static bool IsContextInitialized<TGGDBFContextType>()
			where TGGDBFContextType : IGGDBFContext
		{
			return GetInstance<TGGDBFContextType>() != null;
		}

		public static int[] ConvertToVersion(Version version)
		{
			if(version == null) throw new ArgumentNullException(nameof(version));
			return new int[3] { version.Major, version.Minor, version.Build };
		}

		public static int[] GetContextVersion<TGGDBFContextType>()
			where TGGDBFContextType : IGGDBFContext
		{
			return typeof(TGGDBFContextType)
				.GetCustomAttribute<GeneratedCodeAttribute>()
				.Version
				.Split('.')
				.Select(int.Parse)
				.Take(3)
				.ToArray();
		}

		/// <summary>
		/// Retrieves the model's table name.
		/// WARNING: Highly expensive and hacky reflection is used. Only call this in extreme circumstances.
		/// </summary>
		/// <typeparam name="TModelType"></typeparam>
		/// <returns></returns>
		public static string GetTableName<TModelType>()
		{
			Type tableTypeAttribute = Type.GetType("System.ComponentModel.DataAnnotations.Schema.TableAttribute, System.ComponentModel.Annotations", true);

			Attribute attribute = typeof(TModelType)
				.GetCustomAttribute(tableTypeAttribute, true);

			if (attribute == null)
				throw new InvalidOperationException($"Failed to retrieve Table attribute from {typeof(TModelType).Name} AttributeType: {tableTypeAttribute.Name}");

			return (string) attribute
				.GetType()
				.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
				.GetValue(attribute);
		}
	}
}

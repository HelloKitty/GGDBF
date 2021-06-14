using System;

namespace GGDBF.Generator
{
	/// <summary>
	/// GGDBF's conversion service.
	/// Converts a table Type into a serializable type.
	/// </summary>
	public interface IGGDBFDataConverter
	{
		/// <summary>
		/// Converts the model type to the specified serializable model type.
		/// </summary>
		/// <typeparam name="TModelType">The model type.</typeparam>
		/// <typeparam name="TSerializableModelType">The serializable version of the model.</typeparam>
		/// <param name="model">The model.</param>
		/// <returns>The serializable model.</returns>
		TSerializableModelType Convert<TModelType, TSerializableModelType>(TModelType model)
			where TModelType : class
			where TSerializableModelType : TModelType, IGGDBFSerializable;
	}
}

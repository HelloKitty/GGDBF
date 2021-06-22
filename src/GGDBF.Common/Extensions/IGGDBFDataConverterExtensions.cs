using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGDBF
{
	public static class IGGDBFDataConverterExtensions
	{
		public static TSerializableModelType[] Convert<TModelType, TSerializableModelType>(this IGGDBFDataConverter converter, ICollection<TModelType> models) 
			where TModelType : class 
			where TSerializableModelType : TModelType, IGGDBFSerializable
		{
			if (converter == null) throw new ArgumentNullException(nameof(converter));

			if (models == null)
				return null;

			if (models.Count == 0)
				return Array.Empty<TSerializableModelType>();

			return models
				.Select(converter.Convert<TModelType, TSerializableModelType>)
				.ToArray();
		}
	}
}

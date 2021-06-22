using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using AutoMapper;

namespace GGDBF
{
	public record TypeMapKey(Type ModelType, Type SerializableModelType);

	public sealed class AutoMapperGGDBFDataConverter : IGGDBFDataConverter
	{
		private ConcurrentDictionary<TypeMapKey, IMapper> MappingDictionary { get; } = new();

		public TSerializableModelType Convert<TModelType, TSerializableModelType>(TModelType model) 
			where TModelType : class 
			where TSerializableModelType : TModelType, IGGDBFSerializable
		{
			var typeKey = new TypeMapKey(typeof(TModelType), typeof(TSerializableModelType));
			if (MappingDictionary.ContainsKey(typeKey))
			{
				TSerializableModelType convertedModel = MappingDictionary[typeKey].Map<TModelType, TSerializableModelType>(model);
				convertedModel.Initialize(this);
				return convertedModel;
			}
			else
			{
				CreateMapping<TModelType, TSerializableModelType>(typeKey);
				return Convert<TModelType, TSerializableModelType>(model);
			}
		}

		private void CreateMapping<TModelType, TSerializableModelType>(TypeMapKey key) 
			where TModelType : class where TSerializableModelType : TModelType
		{
			MappingDictionary[key] = new MapperConfiguration(cfg =>
				{
					//See: https://docs.automapper.org/en/stable/Lists-and-arrays.html
					//This is CRITICAL for supporting mapping to collection types!!
					//Generally empty collections are preferrable BUT we use the mapping here
					//only to go from the DataSource model to the Serializable equivalent.
					cfg.AllowNullCollections = true;

					cfg.CreateMap<TModelType, TSerializableModelType>();
				})
				.CreateMapper();
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using GGDBF.Generator;
using AutoMapper;

namespace GGDBF
{
	public record TypeMapKey(Type ModelType, Type SerializableModelType);

	public sealed class AutoMapperGGDBFDataConverter : IGGDBFDataConverter
	{
		private ConcurrentDictionary<TypeMapKey, IMapper> MappingDictionary { get; } = new();

		public TSerializableModelType Convert<TModelType, TSerializableModelType>(TModelType model) 
			where TModelType : class 
			where TSerializableModelType : TModelType
		{
			var typeKey = new TypeMapKey(typeof(TModelType), typeof(TSerializableModelType));
			if (MappingDictionary.ContainsKey(typeKey))
				return MappingDictionary[typeKey].Map<TModelType, TSerializableModelType>(model);
			else
			{
				CreateMapping<TModelType, TSerializableModelType>(typeKey);
				return Convert<TModelType, TSerializableModelType>(model);
			}
		}

		private void CreateMapping<TModelType, TSerializableModelType>(TypeMapKey key) 
			where TModelType : class where TSerializableModelType : TModelType
		{
			MappingDictionary[key] = new MapperConfiguration(cfg => cfg.CreateMap<TModelType, TSerializableModelType>())
				.CreateMapper();
		}
	}
}

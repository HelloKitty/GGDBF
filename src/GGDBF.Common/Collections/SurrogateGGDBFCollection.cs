using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GGDBF
{
	//This exists because of the below warning.
	//Warning: Some serializers will see ICollection and try to serialize it as ICollection<TModelType>
	[DataContract]
	public sealed class SurrogateGGDBFCollection<TKeyType, TModelType>
	{
		/// <summary>
		/// The serialized key values for the collection.
		/// </summary>
		[DataMember(Order = 1)]
		public TKeyType[] References { get; private set; } = Array.Empty<TKeyType>();

		public SurrogateGGDBFCollection()
		{

		}

		public SurrogateGGDBFCollection(TKeyType[] references)
		{
			References = references ?? throw new ArgumentNullException(nameof(references));
		}

		//Exists for the surrogate conversion for some serializers such as protobuf
		public static implicit operator SurrogateGGDBFCollection<TKeyType, TModelType>(SerializableGGDBFCollection<TKeyType, TModelType> collection)
		{
			return collection == null ? null : new SurrogateGGDBFCollection<TKeyType, TModelType>(collection.References);
		}

		//Exists for the surrogate conversion for some serializers such as protobuf
		public static implicit operator SerializableGGDBFCollection<TKeyType, TModelType>(SurrogateGGDBFCollection<TKeyType, TModelType> collection)
		{
			return collection == null ? null : new SerializableGGDBFCollection<TKeyType, TModelType>(collection.References);
		}
	}
}

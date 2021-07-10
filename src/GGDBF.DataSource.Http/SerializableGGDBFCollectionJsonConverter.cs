using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GGDBF
{
	/// <summary>
	/// Represents a JSON.net <see cref="JsonConverter"/> that serializes and deserializes a <see cref="SerializableGGDBFCollection{TKeyType,TModelType}"/>, where
	/// only the internal key array is serialized.
	/// </summary>
	public sealed class SerializableGGDBFCollectionJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsGenericType)
				return false;

			if (objectType.GetGenericTypeDefinition() == typeof(SerializableGGDBFCollection<,>))
				return true;

			return false;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var references = value.GetType()
				.GetProperty(nameof(SerializableGGDBFCollection<object, object>.References))
				.GetValue(value);

			var values = ((IEnumerable)references);

			writer.WriteStartArray();

			foreach (var entry in values)
				serializer.Serialize(writer, entry);

			writer.WriteEndArray();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			//TODO: This is slow
			return GetType()
				.GetMethod(nameof(ReadJsonGeneric), BindingFlags.NonPublic | BindingFlags.Static)
				.MakeGenericMethod(objectType.GenericTypeArguments.First(), objectType.GenericTypeArguments.Last())
				.Invoke(this, new object[] { reader, objectType, existingValue });
		}

		private static object ReadJsonGeneric<TKey, TValue>(JsonReader reader, Type objectType, object existingValue)
		{
			if (objectType.FullName != typeof(SerializableGGDBFCollection<TKey, TValue>).FullName)
				throw new NotSupportedException($"Type {objectType} unexpected, but got a {existingValue.GetType()}");

			var references = JToken.Load(reader)
				.Select(t => t.First.ToObject<TKey>())
				.ToArray();

			return new SerializableGGDBFCollection<TKey, TValue>(references);
		}
	}
}

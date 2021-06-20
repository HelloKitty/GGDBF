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
	//Based on: https://gist.github.com/SteveDunn/e355b98b0dbf5a0209cb8294f7fffe24
	/// <summary>
	/// Represents a JSON.net <see cref="JsonConverter"/> that serialises and deserialises a <see cref="Dictionary{TKey,TValue}"/>, where
	/// JSON.net uses the string representation of dictionary keys, which can cause problems with complex (non-primitive types).
	/// You could override ToString, or add attributes to your type to overcome this problem, but the solution that this type
	/// solves is for when you don't have access to the type being [de]serialised.
	/// This solution was based on this StackOverflow answer (added the deserialisation part): https://stackoverflow.com/a/27043792/28901 
	/// </summary>
	public sealed class GGDBFComplexDictionaryJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (!objectType.IsGenericType)
				return false;

			//We don't need to handle primitive types here.
			if (objectType.GenericTypeArguments[0].IsPrimitive 
			    || objectType.GenericTypeArguments[0] == typeof(string) 
			    || objectType.GenericTypeArguments[0].IsEnum)
				return false;

			if (objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
				return true;

			if (objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
				return true;

			if (objectType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
				return true;

			return false;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var (keyProperty, valueProperty) = GetKeyAndValueProperties(value);

			IEnumerable keys = (IEnumerable)keyProperty.GetValue(value, null);

			var values = ((IEnumerable)valueProperty.GetValue(value, null));
			IEnumerator valueEnumerator = values.GetEnumerator();

			writer.WriteStartArray();

			foreach(object eachKey in keys)
			{
				valueEnumerator.MoveNext();

				writer.WriteStartArray();

				serializer.Serialize(writer, eachKey);
				serializer.Serialize(writer, valueEnumerator.Current);

				writer.WriteEndArray();
			}

			writer.WriteEndArray();
		}

		(PropertyInfo, PropertyInfo) GetKeyAndValueProperties(object value)
		{
			Type type = value.GetType();

			PropertyInfo keyProperty = type.GetProperty("Keys");

			if(keyProperty == null)
				throw new InvalidOperationException($"{value.GetType().Name} was expected to be a {typeof(Dictionary<,>).Name}, and doesn't have a Keys property.");

			var valueProperty = type.GetProperty("Values");

			if(valueProperty == null)
				throw new InvalidOperationException($"{value.GetType().Name} was expected to be a {typeof(Dictionary<,>).Name}, and doesn't have a Values property.");

			return (keyProperty, valueProperty);
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
			if (objectType.FullName != typeof(Dictionary<TKey, TValue>).FullName)
				throw new NotSupportedException($"Type {objectType} unexpected, but got a {existingValue.GetType()}");

			var dictionary = new Dictionary<TKey, TValue>();

			JToken tokens = JToken.Load(reader);

			foreach (var eachToken in tokens)
			{
				TKey key = eachToken[0].ToObject<TKey>();
				TValue value = eachToken[1].ToObject<TValue>();

				dictionary.Add(key, value);
			}

			return dictionary;
		}
	}
}

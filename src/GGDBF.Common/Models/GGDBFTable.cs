using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Serializable Table model type for GGDBF that contains the version, name as well as the table contents
	/// as a <see cref="Dictionary{TKey,TValue}"/>.
	/// </summary>
	/// <typeparam name="TPrimaryKeyType">The key type for the table.</typeparam>
	/// <typeparam name="TModelType">The table model type.</typeparam>
	[DataContract]
	public sealed class GGDBFTable<TPrimaryKeyType, TModelType>
	{
		/// <summary>
		/// GGDBF Version.
		/// (SemVer)
		/// </summary>
		[DataMember(Order = 1)]
		public int[] Version { get; init; } = new int[3];

		/// <summary>
		/// The name of the GGDBF table.
		/// </summary>
		[DataMember(Order = 2)]
		public string TableName { get; init; }

		/// <summary>
		/// Table contents.
		/// (Suggested to never modify this contents)
		/// </summary>
		[DataMember(Order = 3)]
		public Dictionary<TPrimaryKeyType, TModelType> TableData { get; init; }

		/// <summary>
		/// Converts the <see cref="TableData"/> to a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>A read-only <see cref="IReadOnlyDictionary{TKey,TValue}"/> version of <see cref="TableData"/>.</returns>
		public IReadOnlyDictionary<TPrimaryKeyType, TModelType> ToReadOnly()
		{
			return new ReadOnlyDictionary<TPrimaryKeyType, TModelType>(TableData);
		}

		public static int[] ConvertToVersion(Version version)
		{
			if (version == null) throw new ArgumentNullException(nameof(version));
			return new int[3] {version.Major, version.Minor, version.Build};
		}
	}
}

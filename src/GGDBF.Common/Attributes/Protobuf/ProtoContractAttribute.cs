using System;

namespace ProtoBuf
{
	//Hack to allow for non-list serialization for GGDBFCollection
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class ProtoContractAttribute : Attribute
	{
		/// <summary>
		/// If specified, do NOT treat this type as a list, even if it looks like one.
		/// </summary>
		public bool IgnoreListHandling { get; init; }
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GGDBF
{
	/// <summary>
	/// Attribute that indicates to the GGDBF compiler that the specified <see cref="DataType"/> type specified
	/// is requested/required to be compiled and available in the target Type which should represent the database Type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class RequiredDataModelAttribute : Attribute
	{
		/// <summary>
		/// The requested data model type to be compiled and available in the target Type which should represent the database Type.
		/// </summary>
		public Type DataType { get; private set; }

		/// <summary>
		/// Creates a new <see cref="RequiredDataModelAttribute"/> with the provided <see cref="Type"/>.
		/// </summary>
		/// <param name="dataType">The requested data model type.</param>
		public RequiredDataModelAttribute(Type dataType)
		{
			DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
		}
	}
}

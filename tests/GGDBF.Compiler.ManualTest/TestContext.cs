using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{
	[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType3))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType5<int, TestModelType4, TestModelType, short>))]
	public partial class TestContext
	{

	}
}

namespace TestNamespace
{
	[Table("TestDatas")]
	public class TestModelType
	{
		[DatabaseKeyHint]
		public short Id { get; private set; }
	}
}

namespace TestNamespace2
{
	[Table("Test2Datas")]
	public class TestModelType2
	{
		[Key]
		public string Id { get; private set; }
	}

	[Table("Test3DatasWithFK")]
	public class TestModelType3
	{
		[Key]
		public string Id { get; private set; }

		public string ModelId { get; private set; }

		[ForeignKey(nameof(ModelId))]
		public virtual TestModelType2 Model { get; private set; }
	}

	[Table("Test4Datas")]
	public class TestModelType4
	{
		[Key]
		public string Id { get; private set; }

		public virtual ICollection<TestModelType2> ModelCollection { get; private set; }
	}

	[Table("Test5Datas")]
	public class TestModelType5<TKey, TModelType1, TModelType2, TModelType2KeyType>
	{
		[Key]
		public TKey Id { get; private set; }

		public virtual ICollection<TModelType1> ModelCollection { get; private set; }

		public TModelType2KeyType ModelId { get; protected set; }

		[ForeignKey(nameof(ModelId))]
		public virtual TModelType2 Model { get; protected set; }
	}
}

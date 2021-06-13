using System;
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
}

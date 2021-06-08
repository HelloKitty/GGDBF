using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using GGDBF;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{
	[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
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
}

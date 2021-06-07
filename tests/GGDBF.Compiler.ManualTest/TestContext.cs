using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TestNamespace;

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

	}

	[Table("Test2Datas")]
	public class TestModelType2
	{

	}
}

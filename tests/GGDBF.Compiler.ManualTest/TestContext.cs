using System;
using System.Runtime.CompilerServices;
using TestNamespace;

namespace GGDBF
{
	[RequiredDataModel(typeof(string))]
	[RequiredDataModel(typeof(TestModelType))]
	public partial class TestContext
	{

	}
}

namespace TestNamespace
{
	public class TestModelType
	{

	}
}

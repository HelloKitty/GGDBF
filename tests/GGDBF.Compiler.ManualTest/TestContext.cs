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
	[RequiredDataModel(typeof(TestModelTypeUnderscore))]
	public partial class TestContext
	{

	}

	[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType3))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType5<,,,>))]
	public partial class TestContextGeneric<TKey>
	{
		public IReadOnlyDictionary<TKey, TestModelType5<TKey, TestModelType4, TestModelType, short>> Test5Datas { get; init; }
	}
}

namespace TestNamespace
{
	[DataContract]
	[Table("TestDatas")]
	public class TestModelType
	{
		[DataMember(Order = 1)]
		[DatabaseKeyHint]
		public short Id { get; private set; }

		public TestModelType(short id)
		{
			Id = id;
		}

		public TestModelType()
		{
			
		}
	}
}

namespace TestNamespace2
{
	[DataContract]
	[Table("Test2Datas")]
	public class TestModelType2
	{
		[Key]
		[DataMember(Order = 1)]
		public string Id { get; private set; }

		public TestModelType2(string id)
		{
			Id = id;
		}

		public TestModelType2()
		{
			
		}
	}

	[DataContract]
	[Table("Test3DatasWithFK")]
	public class TestModelType3
	{
		[Key]
		[DataMember(Order = 1)]
		public string Id { get; private set; }

		[DataMember(Order = 2)]
		public string ModelId { get; private set; }

		[IgnoreDataMember]
		[ForeignKey(nameof(ModelId))]
		public virtual TestModelType2 Model { get; private set; }

		public TestModelType3(string id, string modelId)
		{
			Id = id;
			ModelId = modelId ?? throw new ArgumentNullException(nameof(modelId));
		}

		public TestModelType3()
		{
			
		}
	}

	[DataContract]
	[Table("Test4Datas")]
	public class TestModelType4
	{
		[Key]
		[DataMember(Order = 1)]
		public string Id { get; private set; }

		[IgnoreDataMember]
		public virtual ICollection<TestModelType2> ModelCollection { get; private set; }

		public TestModelType4(string id, ICollection<TestModelType2> modelCollection)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			ModelCollection = modelCollection ?? throw new ArgumentNullException(nameof(modelCollection));
		}

		public TestModelType4()
		{
			
		}
	}

	[DataContract]
	[Table("Test5Datas")]
	public class TestModelType5<TKey, TModelType1, TModelType2, TModelType2KeyType>
	{
		[Key]
		[DataMember(Order = 1)]
		public TKey Id { get; private set; }

		[IgnoreDataMember]
		public virtual ICollection<TModelType1> ModelCollection { get; private set; }

		[DataMember(Order = 2)]
		public TModelType2KeyType ModelId { get; protected set; }

		[IgnoreDataMember]
		[ForeignKey(nameof(ModelId))]
		public virtual TModelType2 Model { get; protected set; }
	}

	[DataContract]
	[Table("TestDatas_WithUnderScore")]
	public class TestModelTypeUnderscore
	{
		[DataMember(Order = 1)]
		[DatabaseKeyHint]
		public short Id { get; private set; }

		public TestModelTypeUnderscore(short id)
		{
			Id = id;
		}

		public TestModelTypeUnderscore()
		{

		}
	}
}

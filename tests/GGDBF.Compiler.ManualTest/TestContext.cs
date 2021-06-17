using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNameSpace.Extended.Multiple.Words;
using TestNamespace2;

namespace GGDBF
{
	[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType3))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType5<int, TestModelType4, TestModelType, short>))]
	[RequiredDataModel(typeof(TestModelTypeUnderscore))]
	[RequiredDataModel(typeof(TestModelReservedNameTable))]
	[RequiredDataModel(typeof(TestModelType7))]
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

	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType6<>))]
	public partial class TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2>
		where TKey : unmanaged, IConvertible
		where TAnotherType : class, Enum
		where TAnotherType2 : unmanaged
	{
		public IReadOnlyDictionary<TKey, TestModelType6<TKey>> Test6Datas { get; init; }
	}
}

namespace TestNamespace
{
	[DataContract]
	[Table("testDatas")]
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
	[Table("test2_datas")]
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
	[Table("Test6Datas")]
	public class TestModelType6<TKeyType>
		where TKeyType : unmanaged, IConvertible
	{
		[Key]
		[DataMember(Order = 1)]
		public TKeyType Id { get; private set; }

		[DataMember(Order = 2)]
		public string ModelId { get; private set; }

		[ForeignKey(nameof(ModelId))]
		public virtual TestModelType4 Model { get; private set; }
	}

	[DataContract]
	[CompositeKeyHint(nameof(Id1), nameof(Id2))]
	[Table("Test7Datas")]
	public class TestModelType7
	{
		[DataMember(Order = 1)]
		public int Id1 { get; private set; }

		[DataMember(Order = 2)]
		public string Id2 { get; private set; }

		[DataMember(Order = 3)]
		public string ModelId { get; private set; }

		[ForeignKey(nameof(ModelId))]
		public virtual TestModelType4 Model { get; private set; }
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

namespace TestNameSpace.Extended.Multiple.Words
{
	[DataContract]
	[Table("class")]
	public class TestModelReservedNameTable
	{
		[DataMember(Order = 1)]
		[DatabaseKeyHint]
		public short Id { get; private set; }

		public TestModelReservedNameTable(short id)
		{
			Id = id;
		}

		public TestModelReservedNameTable()
		{

		}
	}
}
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
	[RequiredDataModel(typeof(TestModelType8))]
	[RequiredDataModel(typeof(TestModelType9<int, string>))]
	[RequiredDataModel(typeof(TestModelType10<int>))]
	[RequiredDataModel(typeof(TestModelType11<int, short>))]
	public partial class TestContext
	{

	}

	[RequiredDataModel(typeof(TestModelType))]
	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType3))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType5<,,,>))]
	[RequiredDataModel(typeof(TestModelType10<>))]
	[RequiredDataModel(typeof(TestModelType11<,>))]
	public partial class TestContextGeneric<TKey>
	{
		public IReadOnlyDictionary<TKey, TestModelType5<TKey, TestModelType4, TestModelType, short>> Test5Datas { get; init; }

		public IReadOnlyDictionary<TestModelType10Key<TKey>, TestModelType10<TKey>> Test10Datas { get; init; }

		public IReadOnlyDictionary<TestModelType11Key<TKey, TKey>, TestModelType11<TKey, TKey>> Test11Datas { get; init; }
	}

	[RequiredDataModel(typeof(TestModelType2))]
	[RequiredDataModel(typeof(TestModelType4))]
	[RequiredDataModel(typeof(TestModelType6<>))]
	[RequiredDataModel(typeof(TestModelType9<,>))]
	public partial class TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2>
		where TKey : unmanaged, IConvertible
		where TAnotherType : class, Enum
		where TAnotherType2 : unmanaged
	{
		public IReadOnlyDictionary<TKey, TestModelType6<TKey>> Test6Datas { get; init; }

		public IReadOnlyDictionary<TestModelType9Key<TKey, TAnotherType>, TestModelType9<TKey, TAnotherType>> Test9Datas { get; init; }
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

		public TestModelType7(int id1, string id2, string modelId)
		{
			Id1 = id1;
			Id2 = id2;
			ModelId = modelId;
		}

		public TestModelType7()
		{
			
		}
	}

	[DataContract]
	[Table("Test8Datas")]
	public class TestModelType8
	{
		[Key]
		[DataMember(Order = 1)]
		public int Id { get; private set; }

		[DataMember(Order = 2)]
		public int ModelId1 { get; private set; }

		[DataMember(Order = 3)]
		public string ModelId2 { get; private set; }

		[CompositeKeyHint(nameof(ModelId1), nameof(ModelId2))]
		public virtual TestModelType7 Model { get; private set; }

		public TestModelType8(int id, int modelId1, string modelId2)
		{
			Id = id;
			ModelId1 = modelId1;
			ModelId2 = modelId2;
		}

		public TestModelType8()
		{

		}
	}

	[DataContract]
	[CompositeKeyHint(nameof(Id1), nameof(Id2))]
	[Table("Test9Datas")]
	public class TestModelType9<TKeyType1, TKeyType2>
	{
		[DataMember(Order = 1)]
		public TKeyType1 Id1 { get; private set; }

		[DataMember(Order = 2)]
		public TKeyType2 Id2 { get; private set; }

		[DataMember(Order = 3)]
		public string ModelId { get; private set; }

		[ForeignKey(nameof(ModelId))]
		public virtual TestModelType4 Model { get; private set; }

		public TestModelType9(TKeyType1 id1, TKeyType2 id2, string modelId)
		{
			Id1 = id1;
			Id2 = id2;
			ModelId = modelId;
		}

		public TestModelType9()
		{

		}
	}

	[DataContract]
	[CompositeKeyHint(nameof(Id1), nameof(Id2))]
	[Table("Test10Datas")]
	public class TestModelType10<TKeyType1>
	{
		[DataMember(Order = 1)]
		public TKeyType1 Id1 { get; private set; }

		[DataMember(Order = 2)]
		public short Id2 { get; private set; }

		public TestModelType10(TKeyType1 id1, short id2)
		{
			Id1 = id1;
			Id2 = id2;
		}

		public TestModelType10()
		{

		}
	}

	[DataContract]
	[CompositeKeyHint(nameof(Id1), nameof(Id2))]
	[Table("Test11Datas")]
	public class TestModelType11<TKeyType1, TKeyType2>
	{
		[DataMember(Order = 1)]
		public TKeyType1 Id1 { get; private set; }

		[DataMember(Order = 2)]
		public TKeyType2 Id2 { get; private set; }

		[IgnoreDataMember]
		public virtual ICollection<TestModelType10<TKeyType1>> Model { get; private set; }

		public TestModelType11(TKeyType1 id1, TKeyType2 id2, ICollection<TestModelType10<TKeyType1>> model)
		{
			Id1 = id1;
			Id2 = id2;
			Model = model;
		}

		public TestModelType11()
		{
			
		}
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
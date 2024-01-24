using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.80.0")]
public interface ITestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> : IGGDBFContext where TKey : unmanaged, System.IConvertible
	where TAnotherType : class, System.Enum
	where TAnotherType2 : unmanaged
{public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get;}

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get;}

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get;}

public IReadOnlyDictionary<TKey, TestModelType6<TKey>> Test6Datas { get;}

public IReadOnlyDictionary<TestModelType9Key<TKey,TAnotherType>, TestModelType9<TKey,TAnotherType>> Test9Datas { get;}

public IReadOnlyDictionary<TKey, TestModelType15<TKey>> Test15Datas { get;}

}

[GeneratedCodeAttribute("GGDBF", "0.5.80.0")]
public partial class TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> : ITestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> where TKey : unmanaged, System.IConvertible
	where TAnotherType : class, System.Enum
	where TAnotherType2 : unmanaged
{public static TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> Instance { get; private set; }

public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

public static async Task Initialize(IGGDBFDataSource source){

var TestDatasTask = source.RetrieveTableAsync<Int16, TestModelType>(new NameOverrideTableRetrievalConfig<Int16, TestModelType>("TestDatas"));
var Test2DatasTask = source.RetrieveTableAsync<String, TestModelType2>(new NameOverrideTableRetrievalConfig<String, TestModelType2>("Test2Datas"));
var Test4DatasTask = source.RetrieveTableAsync<String, TestModelType4, TestContextGenericConstraints_TestModelType4<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<String, TestModelType4>("Test4Datas"));
var Test6DatasTask = source.RetrieveTableAsync<TKey, TestModelType6<TKey>, TestContextGenericConstraints_TestModelType6<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType6<TKey>>("Test6Datas"));
var Test9DatasTask = source.RetrieveTableAsync<TestModelType9Key<TKey,TAnotherType>, TestModelType9<TKey,TAnotherType>, TestContextGenericConstraints_TestModelType9<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<TestModelType9Key<TKey,TAnotherType>, TestModelType9<TKey,TAnotherType>>("Test9Datas") { KeyResolutionFunction = m => new TestModelType9Key<TKey,TAnotherType>(m.Id1, m.Id2) });
var Test15DatasTask = source.RetrieveTableAsync<TKey, TestModelType15<TKey>, TestContextGenericConstraints_TestModelType15<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType15<TKey>>("Test15Datas"));
Instance = new()
{
TestDatas = await TestDatasTask,
Test2Datas = await Test2DatasTask,
Test4Datas = await Test4DatasTask,
Test6Datas = await Test6DatasTask,
Test9Datas = await Test9DatasTask,
Test15Datas = await Test15DatasTask,
};
}
}
}
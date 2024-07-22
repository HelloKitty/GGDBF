using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
public interface ITestContextGeneric<TKey> : IGGDBFContext
{public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get;}

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get;}

public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get;}

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get;}

public IReadOnlyDictionary<TKey, TestModelType5<TKey,TestModelType4,TestModelType,Int16>> Test5Datas { get;}

public IReadOnlyDictionary<TestModelType10Key<TKey>, TestModelType10<TKey>> Test10Datas { get;}

public IReadOnlyDictionary<TestModelType11Key<TKey,TKey>, TestModelType11<TKey,TKey>> Test11Datas { get;}

public IReadOnlyDictionary<TestModelType13Key<TKey,TKey>, TestModelType13<TKey,TKey>> Test13Datas { get;}

public IReadOnlyDictionary<TKey, TestModelType15<TKey>> Test15Datas { get;}

}

[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
public partial class TestContextGeneric<TKey> : ITestContextGeneric<TKey>
{public static TestContextGeneric<TKey> Instance { get; private set; }

public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

public static async Task Initialize(IGGDBFDataSource source){

var TestDatasTask = source.RetrieveTableAsync<Int16, TestModelType>(new NameOverrideTableRetrievalConfig<Int16, TestModelType>("TestDatas"));
var Test2DatasTask = source.RetrieveTableAsync<String, TestModelType2>(new NameOverrideTableRetrievalConfig<String, TestModelType2>("Test2Datas"));
var Test3DatasWithFKTask = source.RetrieveTableAsync<String, TestModelType3, TestContextGeneric_TestModelType3<TKey>>(new NameOverrideTableRetrievalConfig<String, TestModelType3>("Test3DatasWithFK"));
var Test4DatasTask = source.RetrieveTableAsync<String, TestModelType4, TestContextGeneric_TestModelType4<TKey>>(new NameOverrideTableRetrievalConfig<String, TestModelType4>("Test4Datas"));
var Test5DatasTask = source.RetrieveTableAsync<TKey, TestModelType5<TKey,TestModelType4,TestModelType,Int16>, TestContextGeneric_TestModelType5<TKey>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType5<TKey,TestModelType4,TestModelType,Int16>>("Test5Datas"));
var Test10DatasTask = source.RetrieveTableAsync<TestModelType10Key<TKey>, TestModelType10<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType10Key<TKey>, TestModelType10<TKey>>("Test10Datas") { KeyResolutionFunction = m => new TestModelType10Key<TKey>(m.Id1, m.Id2) });
var Test11DatasTask = source.RetrieveTableAsync<TestModelType11Key<TKey,TKey>, TestModelType11<TKey,TKey>, TestContextGeneric_TestModelType11<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType11Key<TKey,TKey>, TestModelType11<TKey,TKey>>("Test11Datas") { KeyResolutionFunction = m => new TestModelType11Key<TKey,TKey>(m.Id1, m.Id2) });
var Test13DatasTask = source.RetrieveTableAsync<TestModelType13Key<TKey,TKey>, TestModelType13<TKey,TKey>, TestContextGeneric_TestModelType13<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType13Key<TKey,TKey>, TestModelType13<TKey,TKey>>("Test13Datas") { KeyResolutionFunction = m => new TestModelType13Key<TKey,TKey>(m.Id1, m.Id2) });
var Test15DatasTask = source.RetrieveTableAsync<TKey, TestModelType15<TKey>, TestContextGeneric_TestModelType15<TKey>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType15<TKey>>("Test15Datas"));
Instance = new()
{
TestDatas = await TestDatasTask,
Test2Datas = await Test2DatasTask,
Test3DatasWithFK = await Test3DatasWithFKTask,
Test4Datas = await Test4DatasTask,
Test5Datas = await Test5DatasTask,
Test10Datas = await Test10DatasTask,
Test11Datas = await Test11DatasTask,
Test13Datas = await Test13DatasTask,
Test15Datas = await Test15DatasTask,
};
}
}
}
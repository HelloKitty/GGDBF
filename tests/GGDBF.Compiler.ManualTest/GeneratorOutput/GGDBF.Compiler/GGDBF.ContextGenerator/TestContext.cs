using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNamespace2;
using TestNameSpace.Extended.Multiple.Words;

namespace GGDBF
{[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
public interface ITestContext : IGGDBFContext
{public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get;}

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get;}

public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get;}

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType5<Int32,TestModelType4,TestModelType,Int16>> Test5Datas { get;}

public IReadOnlyDictionary<Int16, TestModelTypeUnderscore> TestDatasWithUnderScore { get;}

public IReadOnlyDictionary<Int16, TestModelReservedNameTable> @Class { get;}

public IReadOnlyDictionary<TestModelType7Key, TestModelType7> Test7Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType8> Test8Datas { get;}

public IReadOnlyDictionary<TestModelType9Key<Int32, String>, TestModelType9<Int32,String>> Test9Datas { get;}

public IReadOnlyDictionary<TestModelType10Key<Int32>, TestModelType10<Int32>> Test10Datas { get;}

public IReadOnlyDictionary<TestModelType11Key<Int32, Int16>, TestModelType11<Int32,Int16>> Test11Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType12> Test12Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType14> Test14Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType15<Int32>> Test15Datas { get;}

public IReadOnlyDictionary<String, TestModelType16> Test16Datas { get;}

public IReadOnlyDictionary<Int32, TestModelType17> Test17Datas { get;}

public IReadOnlyDictionary<TestModelType18Key, TestModelType18> Test18Datas { get;}

}

[GeneratedCodeAttribute("GGDBF", "0.5.83.0")]
public partial class TestContext : ITestContext
{public static TestContext Instance { get; private set; }

public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType5<Int32,TestModelType4,TestModelType,Int16>> Test5Datas { get; init; }

public IReadOnlyDictionary<Int16, TestModelTypeUnderscore> TestDatasWithUnderScore { get; init; }

public IReadOnlyDictionary<Int16, TestModelReservedNameTable> @Class { get; init; }

public IReadOnlyDictionary<TestModelType7Key, TestModelType7> Test7Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType8> Test8Datas { get; init; }

public IReadOnlyDictionary<TestModelType9Key<Int32, String>, TestModelType9<Int32,String>> Test9Datas { get; init; }

public IReadOnlyDictionary<TestModelType10Key<Int32>, TestModelType10<Int32>> Test10Datas { get; init; }

public IReadOnlyDictionary<TestModelType11Key<Int32, Int16>, TestModelType11<Int32,Int16>> Test11Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType12> Test12Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType14> Test14Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType15<Int32>> Test15Datas { get; init; }

public IReadOnlyDictionary<String, TestModelType16> Test16Datas { get; init; }

public IReadOnlyDictionary<Int32, TestModelType17> Test17Datas { get; init; }

public IReadOnlyDictionary<TestModelType18Key, TestModelType18> Test18Datas { get; init; }

public static async Task Initialize(IGGDBFDataSource source){

var TestDatasTask = source.RetrieveTableAsync<Int16, TestModelType>(new NameOverrideTableRetrievalConfig<Int16, TestModelType>("TestDatas"));
var Test2DatasTask = source.RetrieveTableAsync<String, TestModelType2>(new NameOverrideTableRetrievalConfig<String, TestModelType2>("Test2Datas"));
var Test3DatasWithFKTask = source.RetrieveTableAsync<String, TestModelType3, TestContext_TestModelType3>(new NameOverrideTableRetrievalConfig<String, TestModelType3>("Test3DatasWithFK"));
var Test4DatasTask = source.RetrieveTableAsync<String, TestModelType4, TestContext_TestModelType4>(new NameOverrideTableRetrievalConfig<String, TestModelType4>("Test4Datas"));
var Test5DatasTask = source.RetrieveTableAsync<Int32, TestModelType5<Int32,TestModelType4,TestModelType,Int16>, TestContext_TestModelType5>(new NameOverrideTableRetrievalConfig<Int32, TestModelType5<Int32,TestModelType4,TestModelType,Int16>>("Test5Datas"));
var TestDatasWithUnderScoreTask = source.RetrieveTableAsync<Int16, TestModelTypeUnderscore>(new NameOverrideTableRetrievalConfig<Int16, TestModelTypeUnderscore>("TestDatasWithUnderScore"));
var @ClassTask = source.RetrieveTableAsync<Int16, TestModelReservedNameTable>(new NameOverrideTableRetrievalConfig<Int16, TestModelReservedNameTable>("Class"));
var Test7DatasTask = source.RetrieveTableAsync<TestModelType7Key, TestModelType7, TestContext_TestModelType7>(new NameOverrideTableRetrievalConfig<TestModelType7Key, TestModelType7>("Test7Datas") { KeyResolutionFunction = m => new TestModelType7Key(m.Id1, m.Id2) });
var Test8DatasTask = source.RetrieveTableAsync<Int32, TestModelType8, TestContext_TestModelType8>(new NameOverrideTableRetrievalConfig<Int32, TestModelType8>("Test8Datas"));
var Test9DatasTask = source.RetrieveTableAsync<TestModelType9Key<Int32, String>, TestModelType9<Int32,String>, TestContext_TestModelType9>(new NameOverrideTableRetrievalConfig<TestModelType9Key<Int32, String>, TestModelType9<Int32,String>>("Test9Datas") { KeyResolutionFunction = m => new TestModelType9Key<Int32, String>(m.Id1, m.Id2) });
var Test10DatasTask = source.RetrieveTableAsync<TestModelType10Key<Int32>, TestModelType10<Int32>>(new NameOverrideTableRetrievalConfig<TestModelType10Key<Int32>, TestModelType10<Int32>>("Test10Datas") { KeyResolutionFunction = m => new TestModelType10Key<Int32>(m.Id1, m.Id2) });
var Test11DatasTask = source.RetrieveTableAsync<TestModelType11Key<Int32, Int16>, TestModelType11<Int32,Int16>, TestContext_TestModelType11>(new NameOverrideTableRetrievalConfig<TestModelType11Key<Int32, Int16>, TestModelType11<Int32,Int16>>("Test11Datas") { KeyResolutionFunction = m => new TestModelType11Key<Int32, Int16>(m.Id1, m.Id2) });
var Test12DatasTask = source.RetrieveTableAsync<Int32, TestModelType12, TestContext_TestModelType12>(new NameOverrideTableRetrievalConfig<Int32, TestModelType12>("Test12Datas"));
var Test14DatasTask = source.RetrieveTableAsync<Int32, TestModelType14, TestContext_TestModelType14>(new NameOverrideTableRetrievalConfig<Int32, TestModelType14>("Test14Datas"));
var Test15DatasTask = source.RetrieveTableAsync<Int32, TestModelType15<Int32>, TestContext_TestModelType15>(new NameOverrideTableRetrievalConfig<Int32, TestModelType15<Int32>>("Test15Datas"));
var Test16DatasTask = source.RetrieveTableAsync<String, TestModelType16, TestContext_TestModelType16>(new NameOverrideTableRetrievalConfig<String, TestModelType16>("Test16Datas"));
var Test17DatasTask = source.RetrieveTableAsync<Int32, TestModelType17, TestContext_TestModelType17>(new NameOverrideTableRetrievalConfig<Int32, TestModelType17>("Test17Datas"));
var Test18DatasTask = source.RetrieveTableAsync<TestModelType18Key, TestModelType18, TestContext_TestModelType18>(new NameOverrideTableRetrievalConfig<TestModelType18Key, TestModelType18>("Test18Datas") { KeyResolutionFunction = m => new TestModelType18Key(m.Id1, m.Id2) });
Instance = new()
{
TestDatas = await TestDatasTask,
Test2Datas = await Test2DatasTask,
Test3DatasWithFK = await Test3DatasWithFKTask,
Test4Datas = await Test4DatasTask,
Test5Datas = await Test5DatasTask,
TestDatasWithUnderScore = await TestDatasWithUnderScoreTask,
@Class = await @ClassTask,
Test7Datas = await Test7DatasTask,
Test8Datas = await Test8DatasTask,
Test9Datas = await Test9DatasTask,
Test10Datas = await Test10DatasTask,
Test11Datas = await Test11DatasTask,
Test12Datas = await Test12DatasTask,
Test14Datas = await Test14DatasTask,
Test15Datas = await Test15DatasTask,
Test16Datas = await Test16DatasTask,
Test17Datas = await Test17DatasTask,
Test18Datas = await Test18DatasTask,
};
}
}
}
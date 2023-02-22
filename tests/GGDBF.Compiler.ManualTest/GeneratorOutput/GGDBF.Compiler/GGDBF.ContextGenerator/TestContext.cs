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
{
    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    public interface ITestContext : IGGDBFContext
    {
        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; }

        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; }

        public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; }

        public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType5<Int32, TestModelType4, TestModelType, Int16>> Test5Datas { get; }

        public IReadOnlyDictionary<Int16, TestModelTypeUnderscore> TestDatasWithUnderScore { get; }

        public IReadOnlyDictionary<Int16, TestModelReservedNameTable> @Class { get; }

        public IReadOnlyDictionary<TestModelType7Key, TestModelType7> Test7Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType8> Test8Datas { get; }

        public IReadOnlyDictionary<TestModelType9Key<Int32, String>, TestModelType9<Int32, String>> Test9Datas { get; }

        public IReadOnlyDictionary<TestModelType10Key<Int32>, TestModelType10<Int32>> Test10Datas { get; }

        public IReadOnlyDictionary<TestModelType11Key<Int32, Int16>, TestModelType11<Int32, Int16>> Test11Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType12> Test12Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType14> Test14Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType15<Int32>> Test15Datas { get; }

        public IReadOnlyDictionary<String, TestModelType16> Test16Datas { get; }

        public IReadOnlyDictionary<Int32, TestModelType17> Test17Datas { get; }

        public IReadOnlyDictionary<TestModelType18Key, TestModelType18> Test18Datas { get; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.1.64.0")]
    public partial class TestContext : ITestContext
    {
        public static TestContext Instance { get; private set; }

        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType5<Int32, TestModelType4, TestModelType, Int16>> Test5Datas { get; init; }

        public IReadOnlyDictionary<Int16, TestModelTypeUnderscore> TestDatasWithUnderScore { get; init; }

        public IReadOnlyDictionary<Int16, TestModelReservedNameTable> @Class { get; init; }

        public IReadOnlyDictionary<TestModelType7Key, TestModelType7> Test7Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType8> Test8Datas { get; init; }

        public IReadOnlyDictionary<TestModelType9Key<Int32, String>, TestModelType9<Int32, String>> Test9Datas { get; init; }

        public IReadOnlyDictionary<TestModelType10Key<Int32>, TestModelType10<Int32>> Test10Datas { get; init; }

        public IReadOnlyDictionary<TestModelType11Key<Int32, Int16>, TestModelType11<Int32, Int16>> Test11Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType12> Test12Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType14> Test14Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType15<Int32>> Test15Datas { get; init; }

        public IReadOnlyDictionary<String, TestModelType16> Test16Datas { get; init; }

        public IReadOnlyDictionary<Int32, TestModelType17> Test17Datas { get; init; }

        public IReadOnlyDictionary<TestModelType18Key, TestModelType18> Test18Datas { get; init; }

        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                TestDatas = await source.RetrieveTableAsync<Int16, TestModelType>(new NameOverrideTableRetrievalConfig<Int16, TestModelType>("TestDatas")),
                Test2Datas = await source.RetrieveTableAsync<String, TestModelType2>(new NameOverrideTableRetrievalConfig<String, TestModelType2>("Test2Datas")),
                Test3DatasWithFK = await source.RetrieveTableAsync<String, TestModelType3, TestContext_TestModelType3>(new NameOverrideTableRetrievalConfig<String, TestModelType3>("Test3DatasWithFK")),
                Test4Datas = await source.RetrieveTableAsync<String, TestModelType4, TestContext_TestModelType4>(new NameOverrideTableRetrievalConfig<String, TestModelType4>("Test4Datas")),
                Test5Datas = await source.RetrieveTableAsync<Int32, TestModelType5<Int32, TestModelType4, TestModelType, Int16>, TestContext_TestModelType5>(new NameOverrideTableRetrievalConfig<Int32, TestModelType5<Int32, TestModelType4, TestModelType, Int16>>("Test5Datas")),
                TestDatasWithUnderScore = await source.RetrieveTableAsync<Int16, TestModelTypeUnderscore>(new NameOverrideTableRetrievalConfig<Int16, TestModelTypeUnderscore>("TestDatasWithUnderScore")),
                @Class = await source.RetrieveTableAsync<Int16, TestModelReservedNameTable>(new NameOverrideTableRetrievalConfig<Int16, TestModelReservedNameTable>("Class")),
                Test7Datas = await source.RetrieveTableAsync<TestModelType7Key, TestModelType7, TestContext_TestModelType7>(new NameOverrideTableRetrievalConfig<TestModelType7Key, TestModelType7>("Test7Datas") { KeyResolutionFunction = m => new TestModelType7Key(m.Id1, m.Id2) }),
                Test8Datas = await source.RetrieveTableAsync<Int32, TestModelType8, TestContext_TestModelType8>(new NameOverrideTableRetrievalConfig<Int32, TestModelType8>("Test8Datas")),
                Test9Datas = await source.RetrieveTableAsync<TestModelType9Key<Int32, String>, TestModelType9<Int32, String>, TestContext_TestModelType9>(new NameOverrideTableRetrievalConfig<TestModelType9Key<Int32, String>, TestModelType9<Int32, String>>("Test9Datas") { KeyResolutionFunction = m => new TestModelType9Key<Int32, String>(m.Id1, m.Id2) }),
                Test10Datas = await source.RetrieveTableAsync<TestModelType10Key<Int32>, TestModelType10<Int32>>(new NameOverrideTableRetrievalConfig<TestModelType10Key<Int32>, TestModelType10<Int32>>("Test10Datas") { KeyResolutionFunction = m => new TestModelType10Key<Int32>(m.Id1, m.Id2) }),
                Test11Datas = await source.RetrieveTableAsync<TestModelType11Key<Int32, Int16>, TestModelType11<Int32, Int16>, TestContext_TestModelType11>(new NameOverrideTableRetrievalConfig<TestModelType11Key<Int32, Int16>, TestModelType11<Int32, Int16>>("Test11Datas") { KeyResolutionFunction = m => new TestModelType11Key<Int32, Int16>(m.Id1, m.Id2) }),
                Test12Datas = await source.RetrieveTableAsync<Int32, TestModelType12, TestContext_TestModelType12>(new NameOverrideTableRetrievalConfig<Int32, TestModelType12>("Test12Datas")),
                Test14Datas = await source.RetrieveTableAsync<Int32, TestModelType14, TestContext_TestModelType14>(new NameOverrideTableRetrievalConfig<Int32, TestModelType14>("Test14Datas")),
                Test15Datas = await source.RetrieveTableAsync<Int32, TestModelType15<Int32>, TestContext_TestModelType15>(new NameOverrideTableRetrievalConfig<Int32, TestModelType15<Int32>>("Test15Datas")),
                Test16Datas = await source.RetrieveTableAsync<String, TestModelType16, TestContext_TestModelType16>(new NameOverrideTableRetrievalConfig<String, TestModelType16>("Test16Datas")),
                Test17Datas = await source.RetrieveTableAsync<Int32, TestModelType17, TestContext_TestModelType17>(new NameOverrideTableRetrievalConfig<Int32, TestModelType17>("Test17Datas")),
                Test18Datas = await source.RetrieveTableAsync<TestModelType18Key, TestModelType18, TestContext_TestModelType18>(new NameOverrideTableRetrievalConfig<TestModelType18Key, TestModelType18>("Test18Datas") { KeyResolutionFunction = m => new TestModelType18Key(m.Id1, m.Id2) }),
            };
        }
    }
}
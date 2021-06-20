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
    [GeneratedCodeAttribute("GGDBF", "0.0.22.0")]
    public interface ITestContext : IGGDBFContext
    {
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

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.22.0")]
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
            };
        }
    }
}
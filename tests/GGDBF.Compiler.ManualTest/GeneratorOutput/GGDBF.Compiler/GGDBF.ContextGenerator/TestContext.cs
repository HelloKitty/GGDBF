using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.4.0")]
    public interface ITestContext : IGGDBFContext
    {
        public IReadOnlyDictionary<short, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<int, TestModelType5<Int32, TestModelType4, TestModelType, Int16>> Test5Datas { get; init; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.4.0")]
    public partial class TestContext : ITestContext
    {
        public static TestContext Instance { get; private set; }

        public IReadOnlyDictionary<short, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<int, TestModelType5<Int32, TestModelType4, TestModelType, Int16>> Test5Datas { get; init; }

        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                TestDatas = await source.RetrieveTableAsync<short, TestModelType>(new NameOverrideTableRetrievalConfig<short, TestModelType>("TestDatas")),
                Test2Datas = await source.RetrieveTableAsync<string, TestModelType2>(new NameOverrideTableRetrievalConfig<string, TestModelType2>("Test2Datas")),
                Test3DatasWithFK = await source.RetrieveTableAsync<string, TestModelType3, TestContext_TestModelType3>(new NameOverrideTableRetrievalConfig<string, TestModelType3>("Test3DatasWithFK")),
                Test4Datas = await source.RetrieveTableAsync<string, TestModelType4, TestContext_TestModelType4>(new NameOverrideTableRetrievalConfig<string, TestModelType4>("Test4Datas")),
                Test5Datas = await source.RetrieveTableAsync<int, TestModelType5<Int32, TestModelType4, TestModelType, Int16>, TestContext_TestModelType5>(new NameOverrideTableRetrievalConfig<int, TestModelType5<Int32, TestModelType4, TestModelType, Int16>>("Test5Datas")),
            };
        }
    }
}
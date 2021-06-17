using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace;
using TestNamespace2;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.10.0")]
    public interface ITestContextGeneric<TKey> : IGGDBFContext
    {
        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<TKey, TestModelType5<TKey, TestModelType4, TestModelType, Int16>> Test5Datas { get; init; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.10.0")]
    public partial class TestContextGeneric<TKey> : ITestContextGeneric<TKey>
    {
        public static TestContextGeneric<TKey> Instance { get; private set; }

        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                TestDatas = await source.RetrieveTableAsync<Int16, TestModelType>(new NameOverrideTableRetrievalConfig<Int16, TestModelType>("TestDatas")),
                Test2Datas = await source.RetrieveTableAsync<String, TestModelType2>(new NameOverrideTableRetrievalConfig<String, TestModelType2>("Test2Datas")),
                Test3DatasWithFK = await source.RetrieveTableAsync<String, TestModelType3, TestContextGeneric_TestModelType3<TKey>>(new NameOverrideTableRetrievalConfig<String, TestModelType3>("Test3DatasWithFK")),
                Test4Datas = await source.RetrieveTableAsync<String, TestModelType4, TestContextGeneric_TestModelType4<TKey>>(new NameOverrideTableRetrievalConfig<String, TestModelType4>("Test4Datas")),
                Test5Datas = await source.RetrieveTableAsync<TKey, TestModelType5<TKey, TestModelType4, TestModelType, Int16>, TestContextGeneric_TestModelType5<TKey>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType5<TKey, TestModelType4, TestModelType, Int16>>("Test5Datas")),
            };
        }
    }
}
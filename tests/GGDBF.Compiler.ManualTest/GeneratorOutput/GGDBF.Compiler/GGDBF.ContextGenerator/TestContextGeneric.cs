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
    [GeneratedCodeAttribute("GGDBF", "0.0.19.0")]
    public interface ITestContextGeneric<TKey> : IGGDBFContext
    {
        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }

        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<String, TestModelType3> Test3DatasWithFK { get; init; }

        public IReadOnlyDictionary<String, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<TKey, TestModelType5<TKey, TestModelType4, TestModelType, Int16>> Test5Datas { get; init; }

        public IReadOnlyDictionary<TestModelType10Key<TKey>, TestModelType10<TKey>> Test10Datas { get; init; }

        public IReadOnlyDictionary<TestModelType11Key<TKey, TKey>, TestModelType11<TKey, TKey>> Test11Datas { get; init; }

        public IReadOnlyDictionary<TestModelType13Key<TKey, TKey>, TestModelType13<TKey, TKey>> Test13Datas { get; init; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.19.0")]
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
                Test10Datas = await source.RetrieveTableAsync<TestModelType10Key<TKey>, TestModelType10<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType10Key<TKey>, TestModelType10<TKey>>("Test10Datas") { KeyResolutionFunction = m => new TestModelType10Key<TKey>(m.Id1, m.Id2) }),
                Test11Datas = await source.RetrieveTableAsync<TestModelType11Key<TKey, TKey>, TestModelType11<TKey, TKey>, TestContextGeneric_TestModelType11<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType11Key<TKey, TKey>, TestModelType11<TKey, TKey>>("Test11Datas") { KeyResolutionFunction = m => new TestModelType11Key<TKey, TKey>(m.Id1, m.Id2) }),
                Test13Datas = await source.RetrieveTableAsync<TestModelType13Key<TKey, TKey>, TestModelType13<TKey, TKey>, TestContextGeneric_TestModelType13<TKey>>(new NameOverrideTableRetrievalConfig<TestModelType13Key<TKey, TKey>, TestModelType13<TKey, TKey>>("Test13Datas") { KeyResolutionFunction = m => new TestModelType13Key<TKey, TKey>(m.Id1, m.Id2) }),
            };
        }
    }
}
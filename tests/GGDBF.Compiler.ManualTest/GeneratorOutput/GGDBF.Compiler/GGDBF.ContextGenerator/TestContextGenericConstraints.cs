using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace2;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.7.0")]
    public interface ITestContextGenericConstraints<TKey> : IGGDBFContext where TKey : unmanaged, System.IConvertible
    {
        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<TKey, TestModelType6<TKey>> Test6Datas { get; init; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.7.0")]
    public partial class TestContextGenericConstraints<TKey> : ITestContextGenericConstraints<TKey> where TKey : unmanaged, System.IConvertible
    {
        public static TestContextGenericConstraints<TKey> Instance { get; private set; }

        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                Test2Datas = await source.RetrieveTableAsync<string, TestModelType2>(new NameOverrideTableRetrievalConfig<string, TestModelType2>("Test2Datas")),
                Test4Datas = await source.RetrieveTableAsync<string, TestModelType4, TestContextGenericConstraints_TestModelType4<TKey>>(new NameOverrideTableRetrievalConfig<string, TestModelType4>("Test4Datas")),
                Test6Datas = await source.RetrieveTableAsync<TKey, TestModelType6<TKey>, TestContextGenericConstraints_TestModelType6<TKey>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType6<TKey>>("Test6Datas")),
            };
        }
    }
}
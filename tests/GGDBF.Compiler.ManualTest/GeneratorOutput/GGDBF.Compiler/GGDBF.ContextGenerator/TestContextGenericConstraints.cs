using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;
using TestNamespace2;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.9.0")]
    public interface ITestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> : IGGDBFContext where TKey : unmanaged, System.IConvertible
       where TAnotherType : class, System.Enum
       where TAnotherType2 : unmanaged
    {
        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public IReadOnlyDictionary<TKey, TestModelType6<TKey>> Test6Datas { get; init; }

    }

    [GeneratedCodeAttribute("GGDBF", "0.0.9.0")]
    public partial class TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> : ITestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> where TKey : unmanaged, System.IConvertible
        where TAnotherType : class, System.Enum
        where TAnotherType2 : unmanaged
    {
        public static TestContextGenericConstraints<TKey, TAnotherType, TAnotherType2> Instance { get; private set; }

        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }

        public IReadOnlyDictionary<string, TestModelType4> Test4Datas { get; init; }

        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                Test2Datas = await source.RetrieveTableAsync<string, TestModelType2>(new NameOverrideTableRetrievalConfig<string, TestModelType2>("Test2Datas")),
                Test4Datas = await source.RetrieveTableAsync<string, TestModelType4, TestContextGenericConstraints_TestModelType4<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<string, TestModelType4>("Test4Datas")),
                Test6Datas = await source.RetrieveTableAsync<TKey, TestModelType6<TKey>, TestContextGenericConstraints_TestModelType6<TKey, TAnotherType, TAnotherType2>>(new NameOverrideTableRetrievalConfig<TKey, TestModelType6<TKey>>("Test6Datas")),
            };
        }
    }
}
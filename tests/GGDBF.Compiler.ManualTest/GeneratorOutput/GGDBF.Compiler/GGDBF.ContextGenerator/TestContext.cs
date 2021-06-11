using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestNamespace;
using TestNamespace2;
namespace GGDBF
{
    public partial class TestContext
    {
        public static TestContext Instance { get; private set; }
        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; init; }
        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; init; }
        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                TestDatas = (await source.RetrieveTableAsync<short, TestNamespace.TestModelType>()).TableData,
                Test2Datas = (await source.RetrieveTableAsync<string, TestNamespace2.TestModelType2>()).TableData,

            };
        }
    }
}
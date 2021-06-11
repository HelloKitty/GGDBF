﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using TestNamespace;
using TestNamespace2;
namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.1.0")]
    public partial class TestContext
    {
        public static TestContext Instance { get; private set; }
        public IReadOnlyDictionary<short, TestModelType> TestDatas { get; init; }
        public IReadOnlyDictionary<string, TestModelType2> Test2Datas { get; init; }
        public static async Task Initialize(IGGDBFDataSource source)
        {
            Instance = new()
            {
                TestDatas = await source.RetrieveTableAsync<short, TestModelType>(new NameOverrideTableRetrievalConfig<short, TestModelType>("TestDatas")),
                Test2Datas = await source.RetrieveTableAsync<string, TestModelType2>(new NameOverrideTableRetrievalConfig<string, TestModelType2>("Test2Datas")),
            };
        }
    }
}
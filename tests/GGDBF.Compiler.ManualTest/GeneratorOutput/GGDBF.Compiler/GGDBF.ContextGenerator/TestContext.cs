using System;
using System.Collections.Generic;
using TestNamespace;
using TestNamespace2;
namespace GGDBF
{
    public partial class TestContext
    {
        public IReadOnlyDictionary<Int16, TestModelType> TestDatas { get; }
        public IReadOnlyDictionary<String, TestModelType2> Test2Datas { get; }
    }
}
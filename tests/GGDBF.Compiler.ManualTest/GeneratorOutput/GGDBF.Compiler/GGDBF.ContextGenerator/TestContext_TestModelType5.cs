using TestNamespace2;
using System;
using TestNamespace;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.26.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType5 : TestModelType5<Int32, TestModelType4, TestModelType, Int16>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }
        public TestContext_TestModelType5() { }

        public void Initialize()
        {
        }
    }
}
using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.1.43.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType9 : TestModelType9<Int32, String>, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestNamespace2.TestModelType4 Model
        {
            get => TestContext.Instance.Test4Datas[base.ModelId];
        }
        public TestContext_TestModelType9() { }

        public void Initialize(IGGDBFDataConverter converter)
        {
        }
    }
}
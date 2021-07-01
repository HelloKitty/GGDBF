using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.26.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType14 : TestModelType14, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType Model
        {
            get => TestContext.Instance.TestDatas[base.ModelId];
        }
        public TestContext_TestModelType14() { }

        public void Initialize()
        {
        }
    }
}
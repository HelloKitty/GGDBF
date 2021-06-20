using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.27.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType12 : TestModelType12, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType2 Model
        {
            get => TestContext.Instance.Test2Datas[base.ModelId];
        }
        public TestContext_TestModelType12() { }

        public void Initialize()
        {
        }
    }
}
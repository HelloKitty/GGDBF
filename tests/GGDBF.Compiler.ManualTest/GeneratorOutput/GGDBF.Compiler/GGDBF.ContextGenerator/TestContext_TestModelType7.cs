using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.15.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType7 : TestModelType7, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType4 Model
        {
            get => TestContext.Instance.Test4Datas[base.ModelId];
        }
        public TestContext_TestModelType7() { }

        public void Initialize()
        {
        }
    }
}
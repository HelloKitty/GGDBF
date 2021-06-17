using TestNamespace2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using GGDBF;

namespace GGDBF
{
    [GeneratedCodeAttribute("GGDBF", "0.0.10.0")]
    [DataContractAttribute]
    public partial class TestContext_TestModelType8 : TestModelType8, IGGDBFSerializable
    {
        [IgnoreDataMemberAttribute]
        public override TestModelType7 Model
        {
            get => TestContext.Instance.Test7Datas[new TestModelType7Key(base.ModelId1, base.ModelId2)];
        }
        public TestContext_TestModelType8() { }

        public void Initialize()
        {
        }
    }
}